/* Shop Page Products Click Handler */

document.addEventListener('DOMContentLoaded', function() {
    // Handle product card clicks and redirect to details page
    const productCardLinks = document.querySelectorAll('.product-card-link');
    
    productCardLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            // Allow normal link behavior
            window.location.href = this.href;
        });
    });

    // Also handle direct card clicks
    const productCards = document.querySelectorAll('.product-card');
    productCards.forEach(card => {
        card.style.cursor = 'pointer';
        card.addEventListener('click', function(e) {
            // Find the parent link and navigate
            const link = this.closest('.product-card-link');
            if (link) {
                window.location.href = link.href;
            }
        });
    });
});
