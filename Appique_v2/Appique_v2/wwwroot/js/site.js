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

// Contact form -> mailto
function handleSubmit(e) {
    e.preventDefault();
    const name    = document.getElementById('f-name').value.trim();
    const email   = document.getElementById('f-email').value.trim();
    const phone   = document.getElementById('f-phone').value.trim();
    const company = document.getElementById('f-company').value.trim();
    const service = document.getElementById('f-service').value;
    const message = document.getElementById('f-message').value.trim();

    if (!name || !email || !message) {
        alert('Please fill in your name, email and project details.');
        return false;
    }

    const body = [
        message, '',
        '---',
        `Name: ${name}`,
        email   ? `Email: ${email}`   : '',
        phone   ? `Phone: ${phone}`   : '',
        company ? `Company: ${company}` : '',
        service ? `Service: ${service}` : '',
    ].filter(Boolean).join('\n');

    const subject = `Project enquiry from ${name}${company ? ', ' + company : ''}`;
    window.location.href = `mailto:hello@appique.co.za?subject=${encodeURIComponent(subject)}&body=${encodeURIComponent(body)}`;
    return false;
}

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
