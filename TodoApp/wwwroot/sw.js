// QuestLife service worker.
// - Pages (HTML) are ALWAYS fetched from the network: they are personal and change constantly, so they are never cached.
//   If the network is down, a friendly offline page is shown instead.
// - Static files (css, js, libraries, icons) are served from the cache and refreshed in the background.
const VERSION = 'ql-v1';
const STATIC_CACHE = `${VERSION}-static`;
const OFFLINE_URL = '/offline.html';
const PRECACHE = [OFFLINE_URL, '/icons/icon-192.png', '/icons/icon-512.png', '/css/site.css', '/js/charts.js', '/js/mobile.js', '/js/pwa.js'];
const STATIC_PREFIXES = ['/css/', '/js/', '/lib/', '/icons/'];

self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(STATIC_CACHE)
            .then(cache => cache.addAll(PRECACHE).catch(() => { /* a missing optional file must not block the install */ }))
            .then(() => self.skipWaiting())
    );
});

self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys()
            .then(keys => Promise.all(keys.filter(k => k.startsWith('ql-') && k !== STATIC_CACHE).map(k => caches.delete(k))))
            .then(() => self.clients.claim())
    );
});

self.addEventListener('fetch', event => {
    const req = event.request;
    if (req.method !== 'GET') return;                       // never touch form posts (login, buying, completing tasks...)
    const url = new URL(req.url);
    if (url.origin !== self.location.origin) return;

    // page navigations: network only, offline page as the fallback
    if (req.mode === 'navigate') {
        event.respondWith(fetch(req).catch(() => caches.match(OFFLINE_URL)));
        return;
    }

    // static assets: cache first, refresh in the background
    if (STATIC_PREFIXES.some(p => url.pathname.startsWith(p))) {
        event.respondWith(
            caches.open(STATIC_CACHE).then(cache =>
                cache.match(req).then(cached => {
                    const network = fetch(req).then(res => {
                        if (res && res.ok) cache.put(req, res.clone());
                        return res;
                    }).catch(() => cached);
                    return cached || network;
                })
            )
        );
    }
    // everything else (JSON, images from elsewhere...) goes straight to the network
});
