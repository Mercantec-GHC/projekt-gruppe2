export function ScrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

export function AttachScrollEvents() {
    let top = true;
    const topThreshold = 400;

    window.addEventListener("scroll", () => {
        const elem = document.getElementById("scroll_top");
        if (elem) {
            if (top && window.scrollY > topThreshold) {
                top = false;
                elem.classList.remove("d-none");
            } else if (!top && window.scrollY <= topThreshold) {
                top = true;
                elem.classList.add("d-none");
            }
        }
    })
}