// Har 30 second mein check karta hai: kisi pending order ka time 1 ghante ke andar hai?
// Ho to "is customer ka order ready karo" ka alert dikhata hai (jab tak site khuli ho).
(function () {
    const modalEl = document.getElementById('readyModal');
    if (!modalEl) return;

    const body = document.getElementById('readyBody');
    const ackBtn = document.getElementById('readyAck');
    const STORE = 'readyAcked';
    let current = [];
    let modal = null;

    function loadAcked() {
        try { return JSON.parse(localStorage.getItem(STORE) || '{}') || {}; } catch (e) { return {}; }
    }
    function saveAcked(o) {
        try { localStorage.setItem(STORE, JSON.stringify(o)); } catch (e) { /* ignore */ }
    }

    const esc = s => String(s ?? '').replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));

    function leftText(m) {
        if (m < 0) return `<span class="badge text-bg-danger">Late (${Math.abs(m)} min)</span>`;
        if (m === 0) return `<span class="badge text-bg-danger">Abhi</span>`;
        return `<span class="badge text-bg-warning">${m} min baqi</span>`;
    }

    function beep() {
        try {
            const ctx = new (window.AudioContext || window.webkitAudioContext)();
            [0, 0.35, 0.7].forEach(t => {
                const o = ctx.createOscillator(), g = ctx.createGain();
                o.type = 'sine'; o.frequency.value = 880;
                o.connect(g); g.connect(ctx.destination);
                g.gain.setValueAtTime(0.25, ctx.currentTime + t);
                o.start(ctx.currentTime + t); o.stop(ctx.currentTime + t + 0.25);
            });
        } catch (e) { /* browser ne awaaz block ki */ }
    }

    function notify(list) {
        try {
            if (!('Notification' in window) || Notification.permission !== 'granted') return;
            list.forEach(o => new Notification('Order ready karo: ' + o.customer, {
                body: `${o.items}\n${o.when} (${o.minutes} min)`,
                tag: 'order-' + o.id
            }));
        } catch (e) { /* ignore */ }
    }

    function render(list) {
        body.innerHTML = list.map(o => `
            <div class="border rounded p-3 mb-2">
                <div class="d-flex justify-content-between flex-wrap gap-2">
                    <div class="fs-5 fw-bold">${esc(o.customer)} <small class="text-muted">#${o.id}</small></div>
                    <div>🕒 <strong>${esc(o.when)}</strong> ${leftText(o.minutes)}</div>
                </div>
                <div class="mt-1">📞 ${esc(o.phone)}</div>
                <div class="mt-1"><strong>${esc(o.items)}</strong><br /><small class="text-muted">${esc(o.details)}</small></div>
            </div>`).join('');
    }

    async function check() {
        try {
            // Koi aur popup khula ho to baad mein dobara try karein.
            if (document.querySelector('.modal.show')) return;

            const res = await fetch('/PendingOrders/Due', { cache: 'no-store' });
            if (!res.ok) return;
            const all = await res.json();
            const acked = loadAcked();
            const fresh = all.filter(o => !acked[o.key]);
            if (!fresh.length) return;

            current = fresh;
            render(fresh);
            modal = modal || bootstrap.Modal.getOrCreateInstance(modalEl);
            modal.show();
            beep();
            notify(fresh);
        } catch (e) { /* network error: agli dafa phir check hoga */ }
    }

    ackBtn.addEventListener('click', () => {
        const acked = loadAcked();
        current.forEach(o => { acked[o.key] = 1; });
        saveAcked(acked);
        current = [];
        if (modal) modal.hide();
    });

    // Browser notification ki ijazat (pehle click par ek dafa).
    document.addEventListener('click', function () {
        try {
            if ('Notification' in window && Notification.permission === 'default') Notification.requestPermission();
        } catch (e) { /* ignore */ }
    }, { once: true });

    setTimeout(check, 1500);
    setInterval(check, 30000);
})();
