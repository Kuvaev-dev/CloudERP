const input = document.getElementById("sidebarSearch");

input?.addEventListener("keyup", function () {
    const filter = input.value.toLowerCase();
    const navItems = document.querySelectorAll("li[data-heading]");
    const headings = document.querySelectorAll("li.nav-heading");

    if (filter === "") {
        navItems.forEach(item => {
            item.style.display = "";
        });
        headings.forEach(heading => {
            heading.style.display = "";
        });
    } else {
        headings.forEach(heading => {
            heading.style.display = "none";
        });

        const visibleHeadings = new Set();

        navItems.forEach(item => {
            const text = item.textContent.toLowerCase();
            if (text.includes(filter)) {
                item.style.display = "";
                const headingId = item.getAttribute("data-heading");
                if (headingId) {
                    visibleHeadings.add(headingId);
                }
            } else {
                item.style.display = "none";
            }
        });

        visibleHeadings.forEach(headingId => {
            const heading = document.querySelector(`li.nav-heading[data-heading-id="${headingId}"]`);
            if (heading) {
                heading.style.display = "";
            }
        });
    }
});