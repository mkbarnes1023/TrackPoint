// Handle office filtering based on location selection
document.addEventListener('DOMContentLoaded', function () {
    const locationSelect = document.getElementById('locationSelect');
    const officeSelect = document.getElementById('officeSelect');
    const allOffices = Array.from(officeSelect.querySelectorAll('option:not(:first-child)'));
    const emptyOption = officeSelect.querySelector('option:first-child');

    function filterOffices() {
        const selectedLocationId = locationSelect.value;

        // Clear current options except the empty option
        officeSelect.innerHTML = '';
        officeSelect.appendChild(emptyOption.cloneNode(true));

        if (selectedLocationId) {
            // Add offices that match the selected location
            const matchingOffices = allOffices.filter(option =>
                option.getAttribute('data-location-id') === selectedLocationId
            );

            matchingOffices.forEach(office => {
                officeSelect.appendChild(office.cloneNode(true));
            });

            // If there are no matching offices, keep it as optional with just the empty option
            if (matchingOffices.length === 0) {
                // Office dropdown is empty but still optional - user can leave it blank
            }
        }

        // Reset office selection
        officeSelect.value = '';
    }

    // Filter offices when location changes
    locationSelect.addEventListener('change', filterOffices);

    // Initial filter on page load
    filterOffices();
});