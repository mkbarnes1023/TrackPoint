(function () {
    console.debug('myAssets.js initializing');

    function formatDate(d) {
        if (!d) return "—";
        var date = d instanceof Date ? d : new Date(d);
        if (isNaN(date)) return "—";
        return date.toLocaleDateString();
    }

    function renderMyAssets() {
        console.debug('renderMyAssets called');
        var container = document.getElementById('myAssets');
        if (!container) {
            console.warn('#myAssets container not found');
            return;
        }

        var dataEl = document.getElementById('my-assets-data');
        var assets = [];

        try {
            if (dataEl) {
                // Prefer textContent but try innerHTML as a fallback
                var raw = (dataEl.textContent || dataEl.innerHTML || '').trim();
                console.debug('my-assets-data raw length:', raw.length);
                if (raw.length) {
                    assets = JSON.parse(raw);
                }
            } else if (Array.isArray(window.myAssets)) {
                assets = window.myAssets;
            }
        } catch (ex) {
            console.error('Failed to parse my-assets JSON', ex, dataEl);
        }

        if (!assets || assets.length === 0) {
            container.innerHTML = '<div class="p-3 text-muted">No assets assigned.</div>';
            return;
        }

        var ul = document.createElement('ul');
        ul.className = 'list-group';

        assets.forEach(function (a) {
            var li = document.createElement('li');
            li.className = 'list-group-item d-flex justify-content-between align-items-center';

            var left = document.createElement('div');
            left.className = 'd-flex flex-column';

            var link = document.createElement('a');
            link.href = '/Asset/AssetView?assetTag=' + encodeURIComponent(a.AssetTag || '');
            link.textContent = a.AssetTag || '(no tag)';
            link.className = 'fw-bold text-decoration-none';

            var meta = document.createElement('small');
            meta.className = 'text-muted';
            var category = a.Category || '—';
            var due = a.DueDate ? formatDate(a.DueDate) : '—';
            meta.textContent = category + ' • Due: ' + due;

            left.appendChild(link);
            left.appendChild(meta);
            li.appendChild(left);
            ul.appendChild(li);
        });

        container.innerHTML = '';
        container.appendChild(ul);
        console.debug('renderMyAssets rendered', assets.length, 'items');
    }

    // expose for manual invocation from console if needed
    window.renderMyAssets = renderMyAssets;

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', renderMyAssets);
    } else {
        renderMyAssets();
    }
})();