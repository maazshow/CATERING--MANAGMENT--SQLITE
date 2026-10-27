(function () {
    document.getElementById('printBtn').addEventListener('click', function () { window.print(); });

    document.getElementById('closeBtn').addEventListener('click', function () {
        if (window.opener) { window.close(); } else { history.back(); }
    });

    // ?print=1 ho to page khulte hi print window aa jati hai.
    if (new URLSearchParams(location.search).get('print') === '1') {
        window.addEventListener('load', function () { setTimeout(function () { window.print(); }, 300); });
    }
})();
