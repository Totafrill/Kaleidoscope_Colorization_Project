﻿document.addEventListener('DOMContentLoaded', function () {

    // Get all "navbar-burger" elements
    var $navbarBurgers = Array.prototype.slice.call(document.querySelectorAll('.navbar-burger'), 0);

    // Check if there are any navbar burgers
    if ($navbarBurgers.length > 0) {

        // Add a click event on each of them
        $navbarBurgers.forEach(function ($el) {
            $el.addEventListener('click', function () {

                // Get the target from the "data-target" attribute
                var target = $el.dataset.target;
                var $target = document.getElementById(target);

                // Toggle the class on both the "navbar-burger" and the "navbar-menu"
                $el.classList.toggle('is-active');
                $target.classList.toggle('is-active');

            });
        });
    }

});

document.querySelectorAll('.navbar-link').forEach(function (navbarLink) {
    navbarLink.addEventListener('click', function () {
        navbarLink.nextElementSibling.classList.toggle('is-hidden-mobile');
    })
});

document.addEventListener('DOMContentLoaded', () => {
    function openModal($el) {
        $el.classList.add('is-active');
    }

    (document.querySelectorAll('.js-modal-trigger') || []).forEach(($trigger) => {
        const modal = $trigger.dataset.target;
        const $target = document.getElementById(modal);

        $trigger.addEventListener('click', () => {
            openModal($target);
        });
    });
});


const fileInput1 = document.querySelector('#file-one input[type=file]');
fileInput1.onchange = () => {
    if (fileInput1.files.length > 0) {
        const fileName = document.querySelector('#file-one .file-name');
        fileName.textContent = fileInput1.files[0].name;
    }
}

const fileInput2 = document.querySelector('#file-two input[type=file]');
fileInput2.onchange = () => {
    if (fileInput2.files.length > 0) {
        const fileName = document.querySelector('#file-two .file-name');
        fileName.textContent = fileInput2.files[0].name;
    }
}

const fileInput3 = document.querySelector('#file-three input[type=file]');
fileInput3.onchange = () => {
    if (fileInput3.files.length > 0) {
        const fileName = document.querySelector('#file-three .file-name');
        fileName.textContent = fileInput3.files[0].name;
    }
}

const fileInput4 = document.querySelector('#file-four input[type=file]');
fileInput4.onchange = () => {
    if (fileInput4.files.length > 0) {
        const fileName = document.querySelector('#file-four .file-name');
        fileName.textContent = fileInput4.files[0].name;
    }
}


