import "./Home.css";

const Home = ({ onViewChange }: { onViewChange?: (view: "todos") => void }) => {
  const githubUrl = "https://github.com/taner04/Taskly";

  const features = [
    {
      icon: "⚡",
      title: "Fast & Simple",
      description: "Add, edit, and organize tasks instantly.",
    },
    {
      icon: "🎯",
      title: "Stay on Track",
      description:
        "Set priorities, deadlines, and reminders that actually help.",
    },
    {
      icon: "📊",
      title: "See Your Progress",
      description: "Visualize what's done and what's next.",
    },
    {
      icon: "🌍",
      title: "Works Everywhere",
      description: "Access your tasks anytime, on any device.",
    },
    {
      icon: "✅",
      title: "Track Completion",
      description:
        "Mark tasks as done and watch your productivity grow with each check.",
    },
    {
      icon: "🔓",
      title: "100% Open Source",
      description:
        "Free, transparent, and community-driven. Contribute and make it yours.",
    },
  ];

  return (
    <div className="home-wrapper">
      {/* Hero Section */}
      <section className="hero-section">
        <h1 className="hero-title">Welcome to Your Smarter To-Do List</h1>
        <p className="hero-subtitle">
          Stay organized, stay focused, and get more done — all in one place.
        </p>
        <p className="hero-description">
          Our interactive to-do app helps you manage tasks effortlessly with
          real-time updates, a clean intuitive design, and best of all — it's
          completely open source and free.
        </p>
      </section>

      {/* Features Section */}
      <section className="features-section">
        <h2 className="section-title">Why You'll Love It</h2>
        <div className="features-grid">
          {features.map((feature, index) => (
            <div key={index} className="feature-card">
              <div className="feature-icon">{feature.icon}</div>
              <h3 className="feature-title">{feature.title}</h3>
              <p className="feature-description">{feature.description}</p>
            </div>
          ))}
        </div>
      </section>

      {/* CTA Section */}
      <section className="cta-section">
        <div className="cta-card">
          <h2 className="cta-title">Get Started</h2>
          <p className="cta-description">
            Create your first list, check something off, and feel that instant
            productivity boost.
          </p>
          <p className="cta-highlight">Your day just got easier.</p>
          <a
            className="cta-button"
            href="#"
            onClick={(e) => {
              e.preventDefault();
              onViewChange?.("todos");
            }}
          >
            Start Building Your List
          </a>
        </div>
      </section>

      {/* Footer Section */}
      <section className="footer-section">
        <p className="footer-tagline">
          Let's get things done — one task at a time.
        </p>
        <a
          className="github-link"
          href={githubUrl}
          target="_blank"
          rel="noopener noreferrer"
        >
          View on GitHub
        </a>
      </section>
    </div>
  );
};

export default Home;
