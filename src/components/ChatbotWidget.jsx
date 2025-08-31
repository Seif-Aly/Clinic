import React, { useEffect, useMemo, useRef, useState } from "react";
import api, { extractErrorMessages } from "../services/api";

const LS_KEY = "chatbot_msgs_ar";

export default function ChatbotWidget() {
  const [open, setOpen] = useState(false);
  const [sending, setSending] = useState(false);
  const [input, setInput] = useState("");
  const [errorBanner, setErrorBanner] = useState("");
  const listRef = useRef(null);

  const [messages, setMessages] = useState(() => {
    try {
      const raw = localStorage.getItem(LS_KEY);
      return raw
        ? JSON.parse(raw)
        : [{ role: "bot", text: "مرحباً! كيف يمكنني مساعدتك؟ 😊" }];
    } catch {
      return [{ role: "bot", text: "مرحباً! كيف يمكنني مساعدتك؟ 😊" }];
    }
  });

  useEffect(() => {
    try {
      localStorage.setItem(LS_KEY, JSON.stringify(messages));
    } catch {}
  }, [messages]);

  useEffect(() => {
    if (!open) return;
    const el = listRef.current;
    if (el) el.scrollTop = el.scrollHeight;
  }, [messages, open, sending]);

  const canSend = useMemo(
    () => input.trim().length > 0 && !sending,
    [input, sending]
  );

  const onToggle = () => {
    setOpen((o) => !o);
    setErrorBanner("");
  };

  const onClear = () => {
    if (!window.confirm("هل تريد مسح المحادثة؟")) return;
    setMessages([
      { role: "bot", text: "تم مسح المحادثة. كيف يمكنني مساعدتك الآن؟ 🧹" },
    ]);
    setErrorBanner("");
  };

  const send = async () => {
    const text = input.trim();
    if (!text || sending) return;
    setSending(true);
    setErrorBanner("");

    setMessages((prev) => [...prev, { role: "user", text }]);
    setInput("");

    try {
      const res = await api.post("/Chat", { message: text });
      const reply = res?.data?.reply ?? "لم أستطع فهم الرد 🤔";
      setMessages((prev) => [...prev, { role: "bot", text: String(reply) }]);
    } catch (err) {
      const errs = extractErrorMessages(err);
      const friendly = errs.join(" • ") || "خطأ في الاتصال أو في الخادم.";
      setMessages((prev) => [...prev, { role: "bot", text: `❌ ${friendly}` }]);
      setErrorBanner(friendly);
    } finally {
      setSending(false);
    }
  };

  const onKeyDown = (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      if (canSend) send();
    }
  };

  return (
    <>
      {!open && (
        <button
          className="btn btn-primary rounded-circle shadow chatbot-fab"
          aria-label="افتح المحادثة"
          onClick={onToggle}
          title="محادثة"
        >
          💬
        </button>
      )}

      {open && (
        <div className="chatbot-panel card shadow-lg" dir="rtl">
          <div className="card-header d-flex justify-content-between align-items-center py-2">
            <strong>المساعد</strong>
            <div className="d-flex align-items-center gap-2">
              <button
                className="btn btn-sm btn-outline-secondary"
                onClick={onClear}
                title="مسح"
              >
                مسح
              </button>
              <button
                className="btn btn-sm btn-outline-dark"
                onClick={onToggle}
                title="إغلاق"
              >
                ✕
              </button>
            </div>
          </div>

          {errorBanner && (
            <div className="alert alert-danger rounded-0 mb-0">
              {errorBanner}
            </div>
          )}

          <div className="chatbot-messages" ref={listRef}>
            {messages.map((m, i) => (
              <div
                key={i}
                className={`chatbot-row ${
                  m.role === "user"
                    ? "justify-content-end"
                    : "justify-content-start"
                }`}
              >
                <div className={`chatbot-bubble ${m.role}`}>{m.text}</div>
              </div>
            ))}
            {sending && (
              <div className="chatbot-row justify-content-start">
                <div className="chatbot-bubble bot">
                  <span className="typing">
                    <span className="dot" /> <span className="dot" />{" "}
                    <span className="dot" />
                  </span>
                </div>
              </div>
            )}
          </div>

          <div className="card-footer p-2">
            <div className="input-group">
              <textarea
                className="form-control"
                placeholder="اكتب رسالتك هنا…"
                value={input}
                onChange={(e) => setInput(e.target.value)}
                onKeyDown={onKeyDown}
                rows={1}
                style={{ resize: "none" }}
              />
              <button
                className="btn btn-primary"
                disabled={!canSend}
                onClick={send}
              >
                {sending ? "يتم الإرسال…" : "إرسال"}
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
}
