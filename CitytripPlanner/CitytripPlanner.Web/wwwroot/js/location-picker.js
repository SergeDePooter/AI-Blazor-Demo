window.locationPicker = (function () {
    // Per-element state keyed by elementId
    const instances = {};

    /**
     * Initialise a Google Maps picker inside the given element.
     * @param {string} elementId - The id of the map container element
     * @param {DotNetObjectReference} dotNetRef - .NET object for JS→Blazor callbacks
     * @param {number|null} lat - Initial latitude (places existing pin)
     * @param {number|null} lng - Initial longitude (places existing pin)
     * @returns {boolean} true if the map was initialised successfully
     */
    function initPicker(elementId, dotNetRef, lat, lng) {
        if (!window.google || !window.google.maps) return false;

        const el = document.getElementById(elementId);
        if (!el) return false;

        const center = (lat != null && lng != null)
            ? { lat, lng }
            : { lat: 48.8566, lng: 2.3522 }; // default: Paris

        const map = new google.maps.Map(el, {
            zoom: lat != null ? 15 : 5,
            center,
            mapTypeControl: false,
            streetViewControl: false,
            fullscreenControl: false
        });

        let marker = null;

        // Place existing pin if coords provided
        if (lat != null && lng != null) {
            marker = new google.maps.Marker({ position: center, map, draggable: true });
        }

        // Click on map to place / move pin
        map.addListener('click', async (event) => {
            const clickLat = event.latLng.lat();
            const clickLng = event.latLng.lng();

            if (!marker) {
                marker = new google.maps.Marker({ position: event.latLng, map, draggable: true });
            } else {
                marker.setPosition(event.latLng);
            }

            instances[elementId].marker = marker;

            // Attempt reverse geocoding
            let name = '';
            if (window.google.maps.Geocoder) {
                try {
                    const geocoder = new google.maps.Geocoder();
                    const response = await geocoder.geocode({ location: event.latLng });
                    if (response.results && response.results.length > 0) {
                        name = response.results[0].formatted_address;
                    }
                } catch (_) {
                    // geocoding failed — name stays empty; user can type manually
                }
            }

            dotNetRef.invokeMethodAsync('OnLocationPicked', clickLat, clickLng, name);
        });

        // Also handle drag end on marker
        if (marker) {
            marker.addListener('dragend', async (event) => {
                const dragLat = event.latLng.lat();
                const dragLng = event.latLng.lng();
                let name = '';
                if (window.google.maps.Geocoder) {
                    try {
                        const geocoder = new google.maps.Geocoder();
                        const response = await geocoder.geocode({ location: event.latLng });
                        if (response.results && response.results.length > 0) {
                            name = response.results[0].formatted_address;
                        }
                    } catch (_) { }
                }
                dotNetRef.invokeMethodAsync('OnLocationPicked', dragLat, dragLng, name);
            });
        }

        instances[elementId] = { map, marker };
        return true;
    }

    /**
     * Destroy the picker instance for the given element.
     * @param {string} elementId
     */
    function destroyPicker(elementId) {
        const instance = instances[elementId];
        if (!instance) return;
        if (instance.marker) instance.marker.setMap(null);
        delete instances[elementId];
    }

    return { initPicker, destroyPicker };
})();
