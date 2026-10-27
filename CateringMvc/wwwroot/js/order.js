(function () {
    const $ = id => document.getElementById(id);
    const form = $('orderForm');
    const d = form.dataset;
    const container = $('itemsContainer');

    const P = { bMeat: +d.bMeat, kMeat: +d.kMeat, bkMeat: +d.bkMeat, rMeat: +d.rMeat, raita: +d.raita, hKg: +d.hKg };
    // Fixed prices by size: TIER[type]["8"|"10"|"12"|"14"]
    let TIER = {};
    try { TIER = JSON.parse(d.tiers); } catch (e) { TIER = {}; }
    const TIER_TYPES = ['Zarda', 'WhiteRice', 'SimpleRice', 'GurChawal', 'ChanaDaig'];
    const tierPrice = (type, kg) => ((TIER[type] || {})[String(kg)]) || 0;

    // Fixed-size items: "Other charges" pre-filled from Price Settings (editable per order).
    const OTHER_DEFAULT = { Biryani: +d.bCook, Zarda: +d.zCook, WhiteRice: +d.wCook, SimpleRice: +d.sCook, GurChawal: +d.gCook, ChanaDaig: +d.cCook };
    // Korma / Beef Korma / Roast: cooking rate per kg meat, Haleem: per item. Taken from Price Settings automatically.
    const COOK_AUTO = { Korma: +d.kCook, BeefKorma: +d.bkCook, Roast: +d.rCook, Haleem: +d.hCook };

    const RICE_OPTIONS = [8, 10, 12, 14];
    const fmt = n => 'Rs ' + Number(n || 0).toLocaleString('en-US', { maximumFractionDigits: 2 });
    const num = el => (el ? parseFloat(el.value) || 0 : 0);

    const TITLES = {
        Biryani: '🍚 Biryani Daig', Zarda: '🍮 Zarda', Korma: '🍲 Korma',
        BeefKorma: '🥩 Beef Korma', Roast: '🍗 Roast', WhiteRice: '🍚 White Rice', SimpleRice: '🍚 Chinese Rice', GurChawal: '🍯 Gor Wala Chawal', Haleem: '🥣 Haleem', ChanaDaig: '🫘 Chana Wala Chawal'
    };

    const RAITA_TYPES = ['Biryani', 'ChanaDaig', 'SimpleRice', 'Korma'];
    function raitaField(v) {
        const x = v && v.raita !== null && v.raita !== undefined ? v.raita : '';
        return `<div class="col-md-6"><label class="form-label">Raita (kg) <span class="text-muted small">optional - Rs ${P.raita}/kg</span></label>
            <input type="number" class="form-control" data-f="RaitaKg" min="0" step="0.5" value="${x}" placeholder="0 = no raita" /></div>`;
    }

    function riceSelect(selected) {
        return `<select class="form-select" data-f="RiceKg">` +
            RICE_OPTIONS.map(r => `<option value="${r}" ${+selected === r ? 'selected' : ''}>${r} kg</option>`).join('') +
            `</select>`;
    }

    function cookingField(col, value, label) {
        return `<div class="${col}"><label class="form-label">${label || 'Other charges (Rs)'}</label>
            <input type="number" class="form-control" data-f="CookingCharge" min="0" step="any" value="${value}" placeholder="0" /></div>`;
    }

    function fieldsFor(type, v, cookValue) {
        const val = x => (x === null || x === undefined ? '' : x);
        if (type === 'Biryani') {
            return `
                <div class="col-md-4"><label class="form-label">Rice (kg)</label>${riceSelect(v.rice || 8)}</div>
                <div class="col-md-4"><label class="form-label">Meat (kg)</label>
                    <input type="number" class="form-control" data-f="MeatKg" min="0.5" step="0.5" value="${val(v.meat)}" required /></div>
                ${cookingField('col-md-4', cookValue)}
                ${raitaField(v)}`;
        }
        if (TIER_TYPES.includes(type)) {
            return `
                <div class="col-md-6"><label class="form-label">Size (kg)</label>${riceSelect(v.rice || 8)}</div>
                ${cookingField('col-md-6', cookValue)}
                ${RAITA_TYPES.includes(type) ? raitaField(v) : ''}`;
        }
        if (type === 'Korma' || type === 'BeefKorma' || type === 'Roast') {
            return `
                <div class="col-md-6"><label class="form-label">Meat (kg)</label>
                    <input type="number" class="form-control" data-f="MeatKg" min="0.5" step="0.5" value="${val(v.meat)}" required /></div>
                ${otherField(v)}
                ${RAITA_TYPES.includes(type) ? raitaField(v) : ''}`;
        }
        // Haleem: kg
        return `
            <div class="col-md-6"><label class="form-label">Haleem (kg)</label>
                <input type="number" class="form-control" data-f="QuantityKg" min="0.5" step="0.5" value="${val(v.qty)}" required /></div>
            ${otherField(v)}`;
    }

    function otherField(v) {
        const x = v && v.other !== null && v.other !== undefined ? v.other : '';
        return `<div class="col-md-6"><label class="form-label">Other charges (Rs)</label>
            <input type="number" class="form-control" data-f="OtherCharge" min="0" step="any" value="${x}" placeholder="0" /></div>`;
    }

    function addItem(type, v, restoring) {
        v = v || {};
        // Naye item par Price Settings wali cooking bhar dein; wapas aaye hue (validation fail) item mein wahi jo likha tha.
        const cookValue = restoring ? (v.cooking === null || v.cooking === undefined ? '' : v.cooking) : (OTHER_DEFAULT[type] ?? '');

        const card = document.createElement('div');
        card.className = 'item-card';
        card.dataset.type = type;
        card.innerHTML = `
            <div class="d-flex justify-content-between align-items-center mb-2">
                <strong class="item-title"></strong>
                <button type="button" class="btn btn-sm btn-outline-danger" data-remove title="Item hata dein"><i class="bi bi-x-lg"></i></button>
            </div>
            <input type="hidden" data-f="ItemType" value="${type}" />
            <div class="row g-2">${fieldsFor(type, v, cookValue)}</div>
            <div class="item-sum d-flex justify-content-between mt-2">
                <span class="text-muted small item-detail"></span>
                <strong class="item-amount">Rs 0</strong>
            </div>`;
        container.appendChild(card);

        // Biryani: meat default rice ke barabar (jab tak khud na badlein).
        if (type === 'Biryani') {
            const rice = card.querySelector('[data-f="RiceKg"]');
            const meat = card.querySelector('[data-f="MeatKg"]');
            if (!meat.value) meat.value = rice.value;
            let touched = !!v.meat;
            meat.addEventListener('input', () => { touched = true; });
            rice.addEventListener('change', () => { if (!touched) meat.value = rice.value; });
        }
        renumber();
    }

    function amountOf(card) {
        const r = baseAmountOf(card);
        const raitaKg = num(card.querySelector('[data-f="RaitaKg"]'));
        if (raitaKg > 0) { r.amount += raitaKg * P.raita; r.detail += ` + Raita ${raitaKg}×${P.raita}`; }
        return r;
    }

    function baseAmountOf(card) {
        const type = card.dataset.type;
        const rice = num(card.querySelector('[data-f="RiceKg"]'));
        const meat = num(card.querySelector('[data-f="MeatKg"]'));
        const qty = num(card.querySelector('[data-f="QuantityKg"]'));
        const cook = num(card.querySelector('[data-f="CookingCharge"]'));
        const other = num(card.querySelector('[data-f="OtherCharge"]'));
        const raitaKg = num(card.querySelector('[data-f="RaitaKg"]'));

        if (type === 'Biryani') {
            const fixed = tierPrice('Biryani', rice);
            return {
                amount: fixed + meat * P.bMeat + cook,
                detail: `${rice} kg fixed ${fixed} + Meat ${meat}×${P.bMeat} + Other ${cook}`
            };
        }
        if (TIER_TYPES.includes(type)) {
            const fixed = tierPrice(type, rice);
            return { amount: fixed + cook, detail: `${rice} kg fixed ${fixed} + Other ${cook}` };
        }
        if (type === 'Korma' || type === 'BeefKorma' || type === 'Roast') {
            const rate = type === 'Korma' ? P.kMeat : type === 'BeefKorma' ? P.bkMeat : P.rMeat;
            const cr = COOK_AUTO[type];
            return { amount: meat * (rate + cr) + other, detail: `Meat ${meat}×${rate} + Cooking ${meat}×${cr}` + (other ? ` + Other ${other}` : '') };
        }
        if (type === 'Haleem') {
            return { amount: qty * P.hKg + COOK_AUTO.Haleem + other, detail: `Haleem ${qty}×${P.hKg} + Cooking ${COOK_AUTO.Haleem}` + (other ? ` + Other ${other}` : '') };
        }
        return { amount: 0, detail: '' };
    }

    function renumber() {
        const cards = container.querySelectorAll('.item-card');
        cards.forEach((card, i) => {
            card.querySelector('.item-title').textContent = `${i + 1}. ${TITLES[card.dataset.type]}`;
            card.querySelectorAll('[data-f]').forEach(el => { el.name = `Items[${i}].${el.dataset.f}`; });
        });
        update();
    }

    function update() {
        let total = 0;
        container.querySelectorAll('.item-card').forEach(card => {
            const r = amountOf(card);
            card.querySelector('.item-amount').textContent = fmt(r.amount);
            card.querySelector('.item-detail').textContent = r.detail;
            total += r.amount;
        });
        $('totalAmount').textContent = fmt(total);

        const advance = parseFloat($('advancePaid').value) || 0;
        const remaining = total - advance;
        const remEl = $('remainingAmount');
        remEl.textContent = fmt(Math.max(0, remaining));
        remEl.className = 'fs-4 fw-bold ' + (advance > total ? 'text-danger' : remaining <= 0 ? 'text-success' : advance > 0 ? 'text-warning' : 'text-danger');
        $('advancePaid').classList.toggle('is-invalid', advance > total);
    }

    document.querySelectorAll('[data-add]').forEach(b =>
        b.addEventListener('click', () => addItem(b.dataset.add)));

    container.addEventListener('input', update);
    container.addEventListener('change', update);
    $('advancePaid').addEventListener('input', update);
    container.addEventListener('click', e => {
        const rm = e.target.closest('[data-remove]');
        if (!rm) return;
        rm.closest('.item-card').remove();
        renumber();
    });

    form.addEventListener('submit', e => {
        const err = $('clientError');
        err.classList.add('d-none');

        if (!container.querySelector('.item-card')) {
            e.preventDefault();
            err.textContent = 'Kam az kam ek item add karein.';
            err.classList.remove('d-none');
            return;
        }
        if (!form.checkValidity()) {
            e.preventDefault();
            form.classList.add('was-validated');
            const bad = form.querySelector(':invalid');
            if (bad) bad.focus();
        }
    });

    // Pehle ke items wapas dikhayen (validation fail ke baad), warna ek Biryani item.
    let initial = [];
    try { initial = JSON.parse($('initialItems').textContent) || []; } catch (e) { initial = []; }
    if (initial.length) initial.forEach(i => addItem(i.type, i, true));
    else addItem('Biryani');
    update();

    // Order save hone ke baad popup oper dikhayen.
    const modal = $('successModal');
    if (modal && modal.dataset.show === '1') {
        bootstrap.Modal.getOrCreateInstance(modal).show();
    }
})();
