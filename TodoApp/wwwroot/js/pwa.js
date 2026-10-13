// PWA support: registers the service worker and offers installation.
//  - Android / desktop Chrome: shows an "Install" button using the browser's install prompt.
//  - iPhone / iPad (Safari): there is no install prompt, so we show the "Share -> Add to Home Screen" steps.
(function () {
    if ('serviceWorker' in navigator) {
        window.addEventListener('load', function () {
            navigator.serviceWorker.register('/sw.js').catch(function (e) { console.warn('Service worker registration failed', e); });
        });
    }

    var banner = document.getElementById('install-banner');
    if (!banner) return;

    var standalone = window.matchMedia('(display-mode: standalone)').matches || window.navigator.standalone === true;
    if (standalone) return;                                          // already installed

    var KEY = 'ql-install-dismissed', DAYS = 14;
    try {
        var t = parseInt(localStorage.getItem(KEY) || '0', 10);
        if (t && Date.now() - t < DAYS * 86400000) return;           // dismissed recently
    } catch (e) { }

    var text = banner.querySelector('.ib-text');
    var action = banner.querySelector('.ib-install');
    var deferred = null;

    function show() { banner.hidden = false; }

    document.getElementById('install-dismiss').addEventListener('click', function () {
        banner.hidden = true;
        try { localStorage.setItem(KEY, String(Date.now())); } catch (e) { }
    });

    // Android / desktop Chrome
    window.addEventListener('beforeinstallprompt', function (e) {
        e.preventDefault();
        deferred = e;
        text.textContent = 'Install QuestLife for a full-screen app experience.';
        action.hidden = false;
        show();
    });
    action.addEventListener('click', function () {
        if (!deferred) return;
        deferred.prompt();
        deferred.userChoice.finally(function () { deferred = null; banner.hidden = true; });
    });
    window.addEventListener('appinstalled', function () { banner.hidden = true; });

    // iOS Safari: manual steps
    var ua = navigator.userAgent;
    var isIos = /iphone|ipad|ipod/i.test(ua) || (navigator.platform === 'MacIntel' && navigator.maxTouchPoints > 1);
    var isSafari = /safari/i.test(ua) && !/crios|fxios|edgios|opios/i.test(ua);
    if (isIos && isSafari) {
        text.innerHTML = 'Install QuestLife: tap <b>Share</b> <span aria-hidden="true">⬆️</span> then <b>Add to Home Screen</b>.';
        action.hidden = true;
        show();
    }
})();
