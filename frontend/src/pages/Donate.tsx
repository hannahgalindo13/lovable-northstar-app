import { FormEvent, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2, HeartHandshake } from "lucide-react";
import { PublicLayout } from "@/components/PublicLayout";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { donatePublic } from "@/services/api";
import { useAuth } from "@/lib/auth";

const PRESET_AMOUNTS = [10, 25, 50, 100];

const Donate = () => {
  const { user } = useAuth();
  const [selectedPreset, setSelectedPreset] = useState<number>(25);
  const [customAmount, setCustomAmount] = useState<string>("");
  const [donorName, setDonorName] = useState("");
  const [email, setEmail] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const resolvedAmount = useMemo(() => {
    if (customAmount.trim().length > 0) {
      return Number(customAmount);
    }
    return selectedPreset;
  }, [customAmount, selectedPreset]);

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setSuccessMessage(null);
    setErrorMessage(null);

    if (!Number.isFinite(resolvedAmount) || resolvedAmount <= 0) {
      setErrorMessage("Please enter an amount greater than $0.");
      return;
    }

    if (!donorName.trim()) {
      setErrorMessage("Please enter your name.");
      return;
    }

    try {
      setIsSubmitting(true);
      await donatePublic({
        amount: Number(resolvedAmount.toFixed(2)),
        donorName: donorName.trim(),
        email: email.trim() || undefined,
      });
      setSuccessMessage("Thank you for your donation!");
      setCustomAmount("");
      setSelectedPreset(25);
      setDonorName("");
      setEmail("");
    } catch {
      setErrorMessage("We could not process your donation. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <PublicLayout>
      <section className="pt-28 pb-16 px-6">
        <div className="max-w-5xl mx-auto grid gap-8 lg:grid-cols-[1.1fr_1fr]">
          <div className="space-y-5">
            <p className="inline-flex items-center gap-2 rounded-full bg-terracotta/10 px-4 py-1.5 text-xs font-medium text-terracotta">
              <HeartHandshake className="h-4 w-4" />
              Your support changes lives
            </p>
            <h1 className="font-display text-4xl font-bold tracking-tight">Make an Impact Today</h1>
            <p className="text-muted-foreground text-lg leading-relaxed">
              Your donation helps North Star Sanctuary provide safe housing, trauma-informed care, and long-term recovery support for survivors.
            </p>
          </div>

          <Card className="shadow-lg border-border/60">
            <CardHeader>
              <CardTitle className="font-display text-2xl">Secure Donation</CardTitle>
            </CardHeader>
            <CardContent>
              <form className="space-y-6" onSubmit={handleSubmit}>
                <div className="space-y-2">
                  <Label>Choose an amount</Label>
                  <div className="grid grid-cols-2 gap-2">
                    {PRESET_AMOUNTS.map((amount) => (
                      <Button
                        key={amount}
                        type="button"
                        variant={selectedPreset === amount && customAmount.length === 0 ? "default" : "outline"}
                        onClick={() => {
                          setSelectedPreset(amount);
                          setCustomAmount("");
                        }}
                        className="h-11"
                      >
                        ${amount}
                      </Button>
                    ))}
                  </div>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="customAmount">Custom amount</Label>
                  <Input
                    id="customAmount"
                    type="number"
                    min="1"
                    step="0.01"
                    value={customAmount}
                    onChange={(e) => setCustomAmount(e.target.value)}
                    placeholder="Enter custom amount"
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="donorName">Name</Label>
                  <Input id="donorName" value={donorName} onChange={(e) => setDonorName(e.target.value)} required />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="donorEmail">Email (optional)</Label>
                  <Input
                    id="donorEmail"
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="you@example.org"
                  />
                </div>

                {successMessage && <p className="text-sm text-green-600">{successMessage}</p>}
                {errorMessage && <p className="text-sm text-destructive">{errorMessage}</p>}

                <Button type="submit" className="w-full h-11" disabled={isSubmitting}>
                  {isSubmitting ? (
                    <span className="inline-flex items-center gap-2">
                      <Loader2 className="h-4 w-4 animate-spin" />
                      Processing donation...
                    </span>
                  ) : (
                    `Donate $${resolvedAmount > 0 ? resolvedAmount : selectedPreset}`
                  )}
                </Button>
              </form>

              {!user?.isAuthenticated && (
                <p className="mt-6 text-sm text-muted-foreground">
                  Want to track your impact?{" "}
                  <Link className="text-terracotta underline-offset-2 hover:underline" to="/login">
                    Sign in
                  </Link>{" "}
                  or{" "}
                  <Link className="text-terracotta underline-offset-2 hover:underline" to="/register">
                    create an account
                  </Link>
                  .
                </p>
              )}
            </CardContent>
          </Card>
        </div>
      </section>
    </PublicLayout>
  );
};

export default Donate;
