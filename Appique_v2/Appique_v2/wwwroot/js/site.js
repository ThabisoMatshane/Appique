// Header scroll effect
const header = document.getElementById('site-header');
window.addEventListener('scroll', () => {
    header.classList.toggle('scrolled', window.scrollY > 20);
}, { passive: true });

// Mobile menu
const menuBtn = document.getElementById('menuBtn');
const mobileNav = document.getElementById('mobileNav');
menuBtn?.addEventListener('click', () => {
    const open = mobileNav.style.display === 'flex';
    mobileNav.style.display = open ? 'none' : 'flex';
    menuBtn.setAttribute('aria-expanded', String(!open));
});
mobileNav?.querySelectorAll('a').forEach(a => {
    a.addEventListener('click', () => {
        mobileNav.style.display = 'none';
        menuBtn?.setAttribute('aria-expanded', 'false');
    });
});

// Contact form -> POST to /Home/Contact
(function () {
    const form = document.getElementById('contact-form');
    if (!form) return;

    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        const btn     = document.getElementById('form-btn');
        const success = document.getElementById('form-success');
        const error   = document.getElementById('form-error');

        success.style.display = 'none';
        error.style.display   = 'none';
        btn.disabled = true;
        btn.textContent = 'Sending…';

        const data = new URLSearchParams(new FormData(form));

        try {
            const res  = await fetch('/Home/Contact', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: data.toString(),
            });
            const json = await res.json();

            if (json.success) {
                success.style.display = 'block';
                form.reset();
            } else {
                error.textContent    = json.message || 'Something went wrong. Please try again.';
                error.style.display  = 'block';
            }
        } catch {
            error.textContent   = 'Could not reach the server. Please try again later.';
            error.style.display = 'block';
        } finally {
            btn.disabled    = false;
            btn.textContent = 'Send message';
        }
    });
})();

// Intersection observer for entrance animations
if ('IntersectionObserver' in window) {
    const els = document.querySelectorAll('.svc, .why-point, .step, .faq-item, .contact-card');
    const obs = new IntersectionObserver((entries) => {
        entries.forEach(e => {
            if (e.isIntersecting) {
                e.target.style.opacity = '1';
                e.target.style.transform = 'translateY(0)';
                obs.unobserve(e.target);
            }
        });
    }, { threshold: 0.1, rootMargin: '0px 0px -40px 0px' });

    els.forEach(el => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(20px)';
        el.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
        obs.observe(el);
    });
}
