// In development, always fetch from the network and do not enable offline support.
// This is becauseCaching would make development more difficult (changes would not be reflected immediately).
self.addEventListener('fetch', () => { });
