/**
 * DataTouch Map Module - Premium Leaflet Integration for Top Locations Widget
 * Features: Pins + Heatmap layers, dark theme tiles, premium tooltips, toggle controls
 */

window.DataTouchMap = (function () {
    let map = null;
    let markersLayer = null;
    let heatmapLayer = null;
    let isInitialized = false;
    let currentView = 'pins'; // 'pins' | 'heatmap' | 'both'
    let userHasInteracted = false;
    let topLocationMarker = null;

    // Premium dark tile style - Stadia Alidade Smooth Dark (more legible than pure black)
    const TILE_URL = 'https://tiles.stadiamaps.com/tiles/alidade_smooth_dark/{z}/{x}/{y}{r}.png';
    const TILE_FALLBACK = 'https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png';
    const TILE_ATTRIBUTION = '&copy; OpenStreetMap contributors';

    // Create premium marker icon
    function createMarkerIcon(isTopLocation, weight, location) {
        const baseSize = isTopLocation ? 24 : Math.min(20, Math.max(10, 8 + Math.sqrt(weight) * 2));
        const color = isTopLocation ? '#6366F1' : '#818CF8';
        const glowSize = isTopLocation ? 20 : 10;
        const borderWidth = isTopLocation ? 3 : 2;

        return L.divIcon({
            className: 'dt-marker-icon',
            html: `
                <div class="dt-marker ${isTopLocation ? 'dt-marker-top' : ''}" 
                     style="
                        width: ${baseSize}px; 
                        height: ${baseSize}px; 
                        background: ${color};
                        border: ${borderWidth}px solid rgba(255,255,255,0.3);
                        box-shadow: 0 0 ${glowSize}px ${color}, 
                                    0 0 ${glowSize * 2}px ${color}50,
                                    0 2px 8px rgba(0,0,0,0.4);
                     ">
                     ${isTopLocation ? '<div class="dt-marker-pulse"></div>' : ''}
                </div>
            `,
            iconSize: [baseSize, baseSize],
            iconAnchor: [baseSize / 2, baseSize / 2]
        });
    }

    // Create premium tooltip content
    function createTooltipContent(point) {
        const deltaClass = (point.delta || 0) >= 0 ? 'positive' : 'negative';
        const deltaSign = (point.delta || 0) >= 0 ? '+' : '';

        return `
            <div class="dt-tooltip-content">
                <div class="dt-tooltip-header">
                    <strong>${point.location}</strong>
                    ${point.isTopLocation ? '<span class="dt-tooltip-badge">TOP</span>' : ''}
                </div>
                <div class="dt-tooltip-stats">
                    <div class="dt-tooltip-stat">
                        <span class="dt-tooltip-label">Interactions</span>
                        <span class="dt-tooltip-value">${point.weight || 0}</span>
                    </div>
                    <div class="dt-tooltip-stat">
                        <span class="dt-tooltip-label">Leads</span>
                        <span class="dt-tooltip-value">${point.leads || 0}</span>
                    </div>
                    <div class="dt-tooltip-stat">
                        <span class="dt-tooltip-label">Conversion</span>
                        <span class="dt-tooltip-value">${(point.conversionRate || 0).toFixed(1)}%</span>
                    </div>
                    ${point.delta !== undefined ? `
                    <div class="dt-tooltip-stat">
                        <span class="dt-tooltip-label">vs Period</span>
                        <span class="dt-tooltip-value ${deltaClass}">${deltaSign}${(point.delta || 0).toFixed(1)}%</span>
                    </div>
                    ` : ''}
                </div>
            </div>
        `;
    }

    // Initialize map
    function init(containerId, centerLat, centerLng, zoom) {
        const container = document.getElementById(containerId);
        if (!container) {
            console.error('[DataTouchMap] Container not found:', containerId);
            return false;
        }

        // Destroy existing map if any
        if (map) {
            destroy();
        }

        try {
            // Ensure container has explicit dimensions
            if (container.offsetHeight < 100) {
                container.style.minHeight = '180px';
            }

            map = L.map(containerId, {
                center: [centerLat || 13.6929, centerLng || -89.2182],
                zoom: zoom || 4,
                zoomControl: false,
                attributionControl: false,
                scrollWheelZoom: true,
                dragging: true,
                minZoom: 2,
                maxZoom: 12
            });

            // Track user interaction
            map.on('movestart', function () {
                userHasInteracted = true;
            });

            // Add zoom control to top-right
            L.control.zoom({ position: 'topright' }).addTo(map);

            // Add tiles with fallback
            const tileLayer = L.tileLayer(TILE_URL, {
                attribution: TILE_ATTRIBUTION,
                maxZoom: 18,
                errorTileUrl: TILE_FALLBACK
            });

            tileLayer.on('tileerror', function () {
                // Switch to fallback tiles
                tileLayer.setUrl(TILE_FALLBACK);
            });

            tileLayer.addTo(map);

            // Create markers layer group
            markersLayer = L.layerGroup().addTo(map);

            // Create heatmap layer (hidden by default)
            if (typeof L.heatLayer !== 'undefined') {
                heatmapLayer = L.heatLayer([], {
                    radius: 35,
                    blur: 25,
                    maxZoom: 10,
                    gradient: {
                        0.2: '#1e3a5f',
                        0.4: '#3b5998',
                        0.6: '#6366F1',
                        0.8: '#818CF8',
                        1.0: '#a5b4fc'
                    }
                });
            }

            isInitialized = true;
            currentView = 'pins';
            userHasInteracted = false;

            // Force size recalculation after render
            setTimeout(() => {
                if (map) map.invalidateSize();
            }, 100);

            // Add view toggle control
            addViewToggle();

            console.log('[DataTouchMap] Initialized successfully');
            return true;
        } catch (error) {
            console.error('[DataTouchMap] Init error:', error);
            return false;
        }
    }

    // Add view toggle control
    function addViewToggle() {
        if (!map) return;

        const ViewToggle = L.Control.extend({
            options: { position: 'topright' },
            onAdd: function () {
                const container = L.DomUtil.create('div', 'dt-view-toggle leaflet-bar');
                container.innerHTML = `
                    <button class="dt-toggle-btn active" data-view="pins" title="Pins">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"/>
                            <circle cx="12" cy="10" r="3"/>
                        </svg>
                    </button>
                    <button class="dt-toggle-btn" data-view="heatmap" title="Heatmap">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <circle cx="12" cy="12" r="10" opacity="0.3"/>
                            <circle cx="12" cy="12" r="6" opacity="0.5"/>
                            <circle cx="12" cy="12" r="3"/>
                        </svg>
                    </button>
                    <button class="dt-toggle-btn" data-view="both" title="Both">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <circle cx="12" cy="12" r="8" opacity="0.4"/>
                            <path d="M12 2v4M12 18v4M2 12h4M18 12h4"/>
                        </svg>
                    </button>
                `;

                L.DomEvent.disableClickPropagation(container);

                container.querySelectorAll('.dt-toggle-btn').forEach(btn => {
                    btn.addEventListener('click', (e) => {
                        e.stopPropagation();
                        const view = btn.getAttribute('data-view');
                        setViewMode(view);
                        container.querySelectorAll('.dt-toggle-btn').forEach(b => b.classList.remove('active'));
                        btn.classList.add('active');
                    });
                });

                return container;
            }
        });

        new ViewToggle().addTo(map);
    }

    // Set view mode
    function setViewMode(mode) {
        if (!map) return;
        currentView = mode;

        // Show/hide layers based on mode
        if (markersLayer) {
            if (mode === 'pins' || mode === 'both') {
                if (!map.hasLayer(markersLayer)) map.addLayer(markersLayer);
            } else {
                if (map.hasLayer(markersLayer)) map.removeLayer(markersLayer);
            }
        }

        if (heatmapLayer) {
            if (mode === 'heatmap' || mode === 'both') {
                if (!map.hasLayer(heatmapLayer)) map.addLayer(heatmapLayer);
            } else {
                if (map.hasLayer(heatmapLayer)) map.removeLayer(heatmapLayer);
            }
        }
    }

    // Update markers without recreating map
    function updateMarkers(mapPoints, preserveView) {
        if (!map || !markersLayer) {
            console.warn('[DataTouchMap] Map not initialized');
            return false;
        }

        try {
            // Clear existing markers
            markersLayer.clearLayers();
            topLocationMarker = null;

            if (!mapPoints || mapPoints.length === 0) {
                return true;
            }

            // Prepare heatmap data
            const heatData = [];

            // Add new markers
            mapPoints.forEach(point => {
                const marker = L.marker([point.latitude, point.longitude], {
                    icon: createMarkerIcon(point.isTopLocation, point.weight, point.location),
                    zIndexOffset: point.isTopLocation ? 1000 : 0
                });

                // Add premium tooltip
                marker.bindTooltip(createTooltipContent(point), {
                    permanent: false,
                    direction: 'top',
                    offset: [0, -10],
                    className: 'dt-premium-tooltip'
                });

                markersLayer.addLayer(marker);

                if (point.isTopLocation) {
                    topLocationMarker = marker;
                }

                // Add to heatmap data
                const intensity = Math.min(1, (point.weight || 1) / 50);
                heatData.push([point.latitude, point.longitude, intensity]);
            });

            // Update heatmap layer
            if (heatmapLayer) {
                heatmapLayer.setLatLngs(heatData);
            }

            // Fit bounds if user hasn't interacted
            if (!preserveView && !userHasInteracted && mapPoints.length > 0) {
                fitBoundsToPoints(mapPoints);
            }

            // Apply current view mode
            setViewMode(currentView);

            console.log('[DataTouchMap] Updated', mapPoints.length, 'markers');
            return true;
        } catch (error) {
            console.error('[DataTouchMap] Update error:', error);
            return false;
        }
    }

    // Fit bounds to all points
    function fitBoundsToPoints(mapPoints) {
        if (!map || !mapPoints || mapPoints.length === 0) return false;

        try {
            if (mapPoints.length === 1) {
                // Single point - center with reasonable zoom
                map.setView([mapPoints[0].latitude, mapPoints[0].longitude], 8);
            } else {
                const bounds = L.latLngBounds(
                    mapPoints.map(p => [p.latitude, p.longitude])
                );
                map.fitBounds(bounds, {
                    padding: [40, 40],
                    maxZoom: 10,
                    animate: true
                });
            }
            return true;
        } catch (error) {
            console.error('[DataTouchMap] FitBounds error:', error);
            return false;
        }
    }

    // Force map size recalculation
    function invalidateSize() {
        if (!map) return false;
        try {
            map.invalidateSize();
            return true;
        } catch (error) {
            return false;
        }
    }

    // Reset user interaction flag (call when data range changes)
    function resetInteraction() {
        userHasInteracted = false;
    }

    // Destroy map instance
    function destroy() {
        if (map) {
            try {
                map.remove();
            } catch (e) { }
            map = null;
            markersLayer = null;
            heatmapLayer = null;
            topLocationMarker = null;
            isInitialized = false;
            userHasInteracted = false;
        }
    }

    // Check if map is ready
    function isReady() {
        return isInitialized && map !== null;
    }

    return {
        init,
        updateMarkers,
        fitBoundsToPoints,
        invalidateSize,
        resetInteraction,
        destroy,
        isReady,
        setViewMode
    };
})();
