// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var state = 'closed';
var hover_state = 'closed'

function toggleSidebar() {

    if (state == 'closed') { // if sidebar is closed
        document.getElementById("toggleImg").style.transform = 'rotate(-180deg)';

        document.documentElement.style.setProperty('--sidebar-width', 'var(--sidebar-open-width)');
        document.documentElement.style.setProperty('--main-left-margin', 'var(--sidebar-open-width)');

        var open_elements = document.getElementsByClassName('sidebar-open');
        for (var i = 0; i < open_elements.length; i++) {
            open_elements[i].style.display = 'flex';
        }

        var closed_elements = document.getElementsByClassName('sidebar-closed');
        for (var i = 0; i < closed_elements.length; i++) {
            closed_elements[i].style.display = 'none';
        }

        state = 'open';
        

    } else if (state == 'open') { // if sidebar is open
        document.getElementById("toggleImg").style.transform = 'rotate(0deg)';

        document.documentElement.style.setProperty('--sidebar-width', 'var(--sidebar-closed-width)');
        document.documentElement.style.setProperty('--main-left-margin', 'var(--sidebar-closed-width)');

        var open_elements = document.getElementsByClassName('sidebar-open');
        for (var i = 0; i < open_elements.length; i++) {
            open_elements[i].style.display = 'none';
        }

        var closed_elements = document.getElementsByClassName('sidebar-closed');
        for (var i = 0; i < closed_elements.length; i++) {
            closed_elements[i].style.display = 'flex';
        }

        state = 'closed';
        
    }
}

function hoverOpenSidebar() {
    if (state == 'closed' && hover_state == 'closed') { // if sidebar is closed and hover is closed

        document.documentElement.style.setProperty('--sidebar-width', 'var(--sidebar-open-width)');

        var open_elements = document.getElementsByClassName('sidebar-open');
        for (var i = 0; i < open_elements.length; i++) {
            open_elements[i].style.display = 'flex';
        }

        var closed_elements = document.getElementsByClassName('sidebar-closed');
        for (var i = 0; i < closed_elements.length; i++) {
            closed_elements[i].style.display = 'none';
        }

        hover_state = 'open';

    }
}

function hoverCloseSidebar() {
    if (state == 'closed' && hover_state == 'open') { // if sidebar is closed and hover is open

        document.documentElement.style.setProperty('--sidebar-width', 'var(--sidebar-closed-width)');

        var open_elements = document.getElementsByClassName('sidebar-open');
        for (var i = 0; i < open_elements.length; i++) {
            open_elements[i].style.display = 'none';
        }

        var closed_elements = document.getElementsByClassName('sidebar-closed');
        for (var i = 0; i < closed_elements.length; i++) {
            closed_elements[i].style.display = 'flex';
        }

        hover_state = 'closed';
    }
}

const table = document.querySelector('table');
const tbody = table.querySelector('tbody');
const maxRows = document.getElementById('maxRows');
var rowsPerPage = parseInt(maxRows.value);
const rows = Array.from(tbody.querySelectorAll('tr'));

function paginate(rows, page, rowsPerPage) {
    const start = (page - 1) * rowsPerPage;
    const end = start + rowsPerPage;
    return rows.slice(start, end);
}

function renderTableRows(rows) {
    tbody.innerHTML = '';
    rows.forEach(row => tbody.appendChild(row));
}

function renderPagination(currentPage, totalPages) {
    const pagination = document.querySelector('.pagination');
    pagination.innerHTML = '';

    const maxButtons = 5; // maximum number of page buttons to show
    let startPage = currentPage - Math.floor(maxButtons / 2);
    if (startPage < 1) {
        startPage = 1;
    }
    let endPage = startPage + maxButtons - 1;
    if (endPage > totalPages) {
        endPage = totalPages;
        startPage = endPage - maxButtons + 1;
        if (startPage < 1) {
            startPage = 1;
        }
    }

    const prevButton = document.createElement('a');
    prevButton.href = '#';
    //prevButton.textContent = '«';
    prevButton.classList.add('pagination-button');
    prevButton.classList.add('prev-button');
    if (currentPage > 1) {
        prevButton.addEventListener('click', () => {
            renderTableRows(paginate(rows, currentPage - 1, rowsPerPage));
            renderPagination(currentPage - 1, totalPages);
        });
    }
    pagination.appendChild(prevButton);

    for (let i = startPage; i <= endPage; i++) {
        const pageButton = document.createElement('a');
        pageButton.href = '#';
        pageButton.textContent = i;
        pageButton.classList.add('pagination-button');
        if (i === currentPage) {
            pageButton.classList.add('active');
        } else {
            pageButton.addEventListener('click', () => {
                renderTableRows(paginate(rows, i, rowsPerPage));
                renderPagination(i, totalPages);
            });
        }
        pagination.appendChild(pageButton);
    }

    const nextButton = document.createElement('a');
    nextButton.href = '#';
    //nextButton.textContent = '»';
    nextButton.classList.add('pagination-button');
    nextButton.classList.add('next-button');
    if (currentPage < totalPages) {
        nextButton.addEventListener('click', () => {
            renderTableRows(paginate(rows, currentPage + 1, rowsPerPage));
            renderPagination(currentPage + 1, totalPages);
        });
    }
    pagination.appendChild(nextButton);
}

renderTableRows(paginate(rows, 1, rowsPerPage));
renderPagination(1, Math.ceil(rows.length / rowsPerPage));

maxRows.addEventListener('change', () => {
    rowsPerPage = parseInt(document.getElementById('maxRows').value);
    renderTableRows(paginate(rows, 1, rowsPerPage));
    renderPagination(1, Math.ceil(rows.length / rowsPerPage));
});

function selectAll() {
    var select = document.getElementById("selectAll").checked;
    var checkbox_elements = document.getElementsByClassName("check-element");
    for (var i = 0; i < checkbox_elements.length; i++) {
        checkbox_elements[i].checked = select;
    }
    check();
}

function unselectAll() {
    var checkbox_elements = document.getElementsByClassName("check-mark");
    for (var i = 0; i < checkbox_elements.length; i++) {
        checkbox_elements[i].checked = false;
    }
    check();
}

function check() {
    var selectedItems = $('.check-element:checked');
    if (selectedItems.length == 0) {
        //document.getElementById("selected-number").style.hidden = 'true';
        document.getElementById("selected-number").innerHTML = "";
        $('#deleteAllButton').prop('disabled', true);
        $('#approveAllButton').prop('disabled', true);
        $('#rejectAllButton').prop('disabled', true);
        $('#unselectButton').prop('disabled', true);
        document.getElementById("unselectButton").style.display = 'none';
    } else {
        //document.getElementById("selected-number").style.hidden = 'false';
        document.getElementById("selected-number").innerHTML = "<label>" + selectedItems.length + " Selected" + "<\label>";
        $('#deleteAllButton').prop('disabled', false);
        $('#approveAllButton').prop('disabled', false);
        $('#rejectAllButton').prop('disabled', false);
        $('#unselectButton').prop('disabled', false);
        document.getElementById("unselectButton").style.display = 'flex';
    }
}

document.getElementById("unselectButton").style.display = 'none';

/*$(document).ready(function () {
    var currentUrl = window.location.pathname;
    $('#sidevar .menu-item').each(function () {
        var linkUrl = $(this).attr('href');
        if (currentUrl === (linkUrl)) {
            $(this).addClass('active');
        }
    });
});*/