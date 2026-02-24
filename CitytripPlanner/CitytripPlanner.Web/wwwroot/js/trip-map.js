window.tripMap = (function () {
    let map = null;
    let markers = [];
    let observer = null;

    function initMap(elementId, initialMarkers) {
        const el = document.getElementById(elementId);
        if (!el || !window.google || !window.google.maps) return false;

        map = new google.maps.Map(el, {
            zoom: 13,
            center: initialMarkers.length > 0
                ? { lat: initialMarkers[0].lat, lng: initialMarkers[0].lng }
                : { lat: 48.8566, lng: 2.3522 },
            mapTypeControl: false,
            streetViewControl: false,
            fullscreenControl: false
        });

        setMarkers(initialMarkers);
        return true;
    }

    function setMarkers(markerData) {
        // Remove existing markers
        markers.forEach(m => m.setMap(null));
        markers = [];

        if (!map || !markerData || markerData.length === 0) return;

        const bounds = new google.maps.LatLngBounds();

        markerData.forEach(data => {
            const position = { lat: data.lat, lng: data.lng };
            const marker = new google.maps.Marker({
                position,
                map,
                title: data.label
            });

            const infoWindow = new google.maps.InfoWindow({ content: data.label });
            marker.addListener('click', () => infoWindow.open(map, marker));

            markers.push(marker);
            bounds.extend(position);
        });

        if (markerData.length === 1) {
            map.setCenter({ lat: markerData[0].lat, lng: markerData[0].lng });
            map.setZoom(15);
        } else {
            map.fitBounds(bounds);
        }
    }

    function updateMarkers(markerData) {
        setMarkers(markerData);
    }

    function destroyMap(elementId) {
        markers.forEach(m => m.setMap(null));
        markers = [];
        map = null;
        if (observer) {
            observer.disconnect();
            observer = null;
        }
    }

    function observeDaySections(dotNetRef) {
        if (observer) observer.disconnect();

        observer = new IntersectionObserver(entries => {
            const visible = entries
                .filter(e => e.isIntersecting)
                .sort((a, b) => b.intersectionRatio - a.intersectionRatio)[0];

            if (visible) {
                const day = parseInt(visible.target.dataset.day, 10);
                if (!isNaN(day)) {
                    dotNetRef.invokeMethodAsync('OnDayChanged', day);
                }
            }
        }, { threshold: 0.3 });

        document.querySelectorAll('[data-day]').forEach(el => observer.observe(el));
    }

    return { initMap, updateMarkers, destroyMap, observeDaySections };
})();
