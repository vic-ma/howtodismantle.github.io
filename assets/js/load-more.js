(function () {
  function initContainer(container) {
    const step = parseInt(container.getAttribute("data-load-more-step") || "12", 10);
    const initial = parseInt(container.getAttribute("data-load-more-initial") || String(step), 10);
    const button = container.querySelector("[data-load-more-button]");
    const items = Array.from(container.querySelectorAll("[data-load-more-item]"));

    if (!button || items.length === 0) return;

    let visible = 0;

    function applyVisibility() {
      items.forEach((el, idx) => {
        const isVisible = idx < visible;
        el.toggleAttribute("data-load-more-hidden", !isVisible);
      });

      const remaining = Math.max(0, items.length - visible);
      if (remaining <= 0) {
        button.setAttribute("hidden", "hidden");
      } else {
        button.removeAttribute("hidden");
        const label = button.getAttribute("data-load-more-label") || button.textContent || "Load more";
        button.textContent = `${label} (${Math.min(step, remaining)} of ${remaining})`;
      }
    }

    visible = Math.min(initial, items.length);
    applyVisibility();

    button.addEventListener("click", () => {
      visible = Math.min(items.length, visible + step);
      applyVisibility();
    });
  }

  document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll("[data-load-more-container]").forEach(initContainer);
  });
})();





