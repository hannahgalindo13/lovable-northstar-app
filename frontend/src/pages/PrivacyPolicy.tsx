import { PublicLayout } from "@/components/PublicLayout";
import { motion } from "framer-motion";

const Reveal = ({ children, className = "", delay: d = 0 }: { children: React.ReactNode; className?: string; delay?: number }) => (
  <motion.div
    initial={{ opacity: 0, y: 30 }}
    whileInView={{ opacity: 1, y: 0 }}
    viewport={{ once: true }}
    transition={{ duration: 0.7, delay: d, ease: [0.22, 1, 0.36, 1] }}
    className={className}
  >
    {children}
  </motion.div>
);

const sections = [
  { id: "collection", title: "Information We Collect", content: `We collect account identity data (name, email, username), donation records (amounts, dates, channels), communication records (messages and support requests), and technical usage data (IP address, browser, pages visited, and security logs). We collect only data necessary to operate the service and protect accounts.` },
  { id: "use", title: "Why We Collect It", content: `We process data to authenticate users, provide donor dashboards, process and reconcile donations, produce impact reporting, detect abuse, comply with legal obligations, and respond to support requests. We never sell personal data.` },
  { id: "lawful-basis", title: "Legal Basis for Processing", content: `We process personal data under GDPR lawful bases including consent, performance of a contract, legitimate interests (service security and fraud prevention), and legal obligations (financial reporting and compliance).` },
  { id: "retention", title: "Data Retention", content: `We keep personal data only as long as needed for the purposes above, required accounting periods, and legal obligations. When retention periods expire, data is deleted or anonymized.` },
  { id: "protection", title: "Data Protection & Security", content: `We apply encrypted transport (HTTPS/TLS), access controls, role-based authorization, audit logging, and least-privilege administrative access. Passwords are stored using one-way cryptographic hashing via ASP.NET Identity.` },
  { id: "cookies", title: "Cookies & Tracking", content: `Essential cookies are used for core site functionality. Optional analytics cookies are used only with your consent. You can accept or reject optional cookies through the cookie banner and change your choice by clearing browser storage.` },
  { id: "rights", title: "Your GDPR Rights", content: `You may request access, correction, deletion, restriction, portability, and objection to processing. To submit a data request, contact privacy@northstarsanctuary.org and include your account email plus request type. We verify identity before fulfilling requests and respond within one month.` },
  { id: "deletion", title: "How to Request Deletion", content: `To request account or donation data deletion, email privacy@northstarsanctuary.org with subject line "Data Deletion Request", your account email, and any relevant donation references. We will confirm receipt, verify identity, and delete or anonymize eligible records in line with legal retention requirements.` },
  { id: "changes", title: "Changes to This Policy", content: `We may update this Privacy Policy periodically. Material changes will be communicated through our website. The date of the most recent revision is always noted at the top of this page.` },
];

const PrivacyPolicy = () => (
  <PublicLayout>
    <section className="pt-32 lg:pt-44 pb-28 gradient-cream-warm">
      <div className="max-w-3xl mx-auto px-6">
        <Reveal>
          <p className="font-body text-[11px] font-medium uppercase tracking-[0.3em] text-terracotta mb-6">Legal</p>
          <h1 className="font-display text-[clamp(2rem,4vw,3.5rem)] font-bold text-foreground leading-[1.1] mb-4">Privacy Policy</h1>
          <p className="font-body text-sm text-muted-foreground">Last updated: March 15, 2024</p>
        </Reveal>
      </div>
    </section>

    <section className="pb-28 bg-background">
      <div className="max-w-3xl mx-auto px-6 -mt-8">
        {/* TOC */}
        <Reveal>
          <nav className="bg-card rounded-2xl p-8 mb-16">
            <p className="font-body text-[11px] font-medium uppercase tracking-[0.2em] text-terracotta mb-4">Contents</p>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-1.5">
              {sections.map((s, i) => (
                <a key={s.id} href={`#${s.id}`} className="text-sm font-body text-muted-foreground hover:text-terracotta transition-colors py-1">
                  {i + 1}. {s.title}
                </a>
              ))}
            </div>
          </nav>
        </Reveal>

        <div className="space-y-16">
          {sections.map((s, i) => (
            <Reveal key={s.id}>
              <div id={s.id}>
                <h2 className="font-display text-xl font-semibold text-foreground mb-4">
                  {i + 1}. {s.title}
                </h2>
                <p className="font-body text-sm text-muted-foreground leading-[1.8]">{s.content}</p>
              </div>
            </Reveal>
          ))}
        </div>

        <Reveal>
          <div className="mt-20 bg-terracotta/8 rounded-2xl p-8">
            <h3 className="font-display text-lg font-semibold text-foreground mb-2">Questions?</h3>
            <p className="font-body text-sm text-muted-foreground">
              Contact our Privacy Officer at{" "}
              <a href="mailto:privacy@northstarsanctuary.org" className="text-terracotta hover:underline">
                privacy@northstarsanctuary.org
              </a>
            </p>
          </div>
        </Reveal>
      </div>
    </section>
  </PublicLayout>
);

export default PrivacyPolicy;
