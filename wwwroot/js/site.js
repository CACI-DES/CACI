window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
};

// Mobile Navigation Toggle and Interactions
function initMobileNav() {
    const menuBtn = document.querySelector(".mobile-menu-btn");
    const mainNav = document.querySelector(".main-nav");
    
    if (menuBtn && mainNav) {
        // Toggle mobile menu
        menuBtn.onclick = function (e) {
            e.stopPropagation();
            mainNav.classList.toggle("active");
        };
    }

    // Toggle dropdown submenus on click for mobile/tablet screens
    const toggles = document.querySelectorAll(".dropdown-toggle");
    toggles.forEach(function (toggle) {
        toggle.onclick = function (e) {
            // Only handle click toggle if menuBtn is visible (indicates mobile/tablet layout active)
            const menuBtn = document.querySelector(".mobile-menu-btn");
            const isMobileLayout = menuBtn && window.getComputedStyle(menuBtn).display !== "none";
            if (!isMobileLayout) return;

            e.preventDefault();
            e.stopPropagation();
            
            const submenu = toggle.nextElementSibling;
            const chevron = toggle.querySelector("i");
            
            // Close other open submenus
            document.querySelectorAll(".submenu").forEach(function (sub) {
                if (sub !== submenu) {
                    sub.classList.remove("mobile-open");
                }
            });
            document.querySelectorAll(".dropdown-toggle i").forEach(function (chv) {
                if (chv !== chevron) {
                    chv.classList.remove("rotate-180");
                }
            });

            if (submenu) {
                submenu.classList.toggle("mobile-open");
            }
            if (chevron) {
                chevron.classList.toggle("rotate-180");
            }
        };
    });

    // Close mobile nav when clicking outside
    document.onclick = function (e) {
        if (mainNav && !mainNav.contains(e.target) && !e.target.closest(".mobile-menu-btn")) {
            mainNav.classList.remove("active");
            document.querySelectorAll(".submenu").forEach(function (sub) {
                sub.classList.remove("mobile-open");
            });
            document.querySelectorAll(".dropdown-toggle i").forEach(function (chv) {
                chv.classList.remove("rotate-180");
            });
        }
    };
}

// Bind navbar initialization
if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initMobileNav);
} else {
    initMobileNav();
}

window.scrollToTodaySchedule = function (elementId) {
    if (!elementId) return;
    const elem = document.getElementById(elementId);
    if (elem) {
        elem.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
};




