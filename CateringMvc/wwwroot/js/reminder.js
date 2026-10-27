(function () {
    const modal = document.getElementById('reminderModal');
    if (!modal) return;

    // Order page par success popup khula ho to reminder agli dafa dikhayen.
    const success = document.getElementById('successModal');
    if (success && success.dataset.show === '1') return;

    const today = modal.dataset.today;
    const key = 'reminderShownOn';

    let shownOn = null;
    try { shownOn = localStorage.getItem(key); } catch (e) { /* ignore */ }
    if (shownOn === today) return;   // aaj pehle hi dikha chuke hain

    try { localStorage.setItem(key, today); } catch (e) { /* ignore */ }
    bootstrap.Modal.getOrCreateInstance(modal).show();
})();
