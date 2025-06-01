// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    // Handle Add to Watchlist button clicks
    document.querySelectorAll('.add-to-watchlist').forEach(button => {
        button.addEventListener('click', function () {
            const movieId = this.getAttribute('data-movie-id');

            // Create a form to submit
            const form = document.createElement('form');
            form.method = 'POST';
            form.action = '/Watchlist/AddToWatchlist';
            form.style.display = 'none';

            // Add movie ID input
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'movieId';
            input.value = movieId;
            form.appendChild(input);

            // Add anti-forgery token if available
            const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
            if (tokenInput) {
                const tokenClone = tokenInput.cloneNode(true);
                form.appendChild(tokenClone);
            }

            // Add form to document and submit it
            document.body.appendChild(form);
            form.submit();
        });
    });

    // Handle Remove from Watchlist button clicks
    document.querySelectorAll('.remove-from-watchlist').forEach(button => {
        button.addEventListener('click', function () {
            const movieId = this.getAttribute('data-movie-id');

            // Create a form to submit
            const form = document.createElement('form');
            form.method = 'POST';
            form.action = '/Watchlist/RemoveFromWatchlist';
            form.style.display = 'none';

            // Add movie ID input
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'movieId';
            input.value = movieId;
            form.appendChild(input);

            // Add anti-forgery token if available
            const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
            if (tokenInput) {
                const tokenClone = tokenInput.cloneNode(true);
                form.appendChild(tokenClone);
            }

            // Add form to document and submit it
            document.body.appendChild(form);
            form.submit();
        });
    });
});
