import { AIInsightCard } from "@/components/AIInsightCard";
import { Brain } from "lucide-react";
import { motion } from "framer-motion";
import { useEffect, useState } from "react";
import { getImpactData } from "@/services/api";

const InsightsPage = () => {
  const [insights, setInsights] = useState<
    Array<{ type: string; title: string; description: string; urgency: "low" | "medium" | "high"; action: string }>
  >([]);

  useEffect(() => {
    const load = async () => {
      try {
        const impact = await getImpactData();
        setInsights(
          impact.programOutcomes.map((outcome) => ({
            type: "outcome",
            title: `${outcome.program} performance`,
            description: `${outcome.program} currently reports a ${outcome.rate}% success outcome.`,
            urgency: outcome.rate < 40 ? "high" : outcome.rate < 70 ? "medium" : "low",
            action: "Review program",
          })),
        );
      } catch (error) {
        console.error(error);
      }
    };

    load();
  }, []);

  return (
    <div className="space-y-8">
      <div className="flex items-center gap-3">
        <div className="w-10 h-10 rounded-xl bg-terracotta/10 flex items-center justify-center">
          <Brain className="w-5 h-5 text-terracotta" />
        </div>
        <div>
          <h1 className="font-display text-2xl lg:text-3xl font-bold text-foreground">AI Insights</h1>
          <p className="font-body text-sm text-muted-foreground mt-0.5">
            Machine learning–powered recommendations.
          </p>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
        {insights.map((insight, i) => (
          <motion.div key={i} initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: i * 0.06 }}>
            <AIInsightCard {...insight} />
          </motion.div>
        ))}
      </div>
    </div>
  );
};

export default InsightsPage;
