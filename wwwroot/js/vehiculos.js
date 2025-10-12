document.addEventListener('DOMContentLoaded', () => {
    const filterInput = document.getElementById('filterInput');
    const table = document.getElementById('vehiculosTable');
    const rows = table.querySelectorAll('tbody tr');

    filterInput.addEventListener('input', () => {
        const filter = filterInput.value.toLowerCase();

        rows.forEach(row => {
            const text = row.innerText.toLowerCase();
            row.style.display = text.includes(filter) ? '' : 'none';
        });
    });
});


