// Pending aur Remaining Payments pages: "Done" aur "Payment" ke popups bharta hai.
(function () {
    function fmt(n) { return 'Rs ' + Number(n || 0).toLocaleString('en-US', { maximumFractionDigits: 2 }); }
    function show(id) { bootstrap.Modal.getOrCreateInstance(document.getElementById(id)).show(); }

    document.addEventListener('click', function (e) {
        const done = e.target.closest('[data-done]');
        if (done) {
            document.getElementById('doneId').value = done.dataset.id;
            document.getElementById('doneNo').textContent = '#' + done.dataset.id;
            document.getElementById('doneCustomer').textContent = done.dataset.customer;
            document.getElementById('doneRemaining').textContent = fmt(done.dataset.remaining);
            show('doneModal');
            return;
        }

        const rec = e.target.closest('[data-receive]');
        if (rec) {
            document.getElementById('receiveId').value = rec.dataset.id;
            document.getElementById('receiveNo').textContent = '#' + rec.dataset.id;
            document.getElementById('receiveCustomer').textContent = rec.dataset.customer;
            document.getElementById('receiveRemaining').textContent = fmt(rec.dataset.remaining);
            document.getElementById('receiveReturn').value = location.pathname + location.search;
            const amt = document.getElementById('receiveAmount');
            amt.value = '';
            amt.max = rec.dataset.remaining;
            amt.placeholder = rec.dataset.remaining;
            show('receiveModal');
        }
    });
})();
