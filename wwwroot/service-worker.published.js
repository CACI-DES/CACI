self.cacheName = 'caci-v1';
self.filesToCache = [
    './',
    './css/app.css',
    './CACI_Blazor.styles.css',
    './images/logos/AllTheWord_NOTextLogo.png',
    './manifest.json',
    './_framework/blazor.web.js'
];

self.addEventListener('install', (event) => {
    event.waitUntil(
        caches.open(self.cacheName).then((cache) => {
            return cache.addAll(self.filesToCache);
        })
    );
});

self.addEventListener('fetch', (event) => {
    event.respondWith(
        caches.match(event.request).then((response) => {
            return response || fetch(event.request);
        })
    );
});
