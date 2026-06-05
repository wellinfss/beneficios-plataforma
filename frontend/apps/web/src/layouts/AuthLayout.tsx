interface AuthLayoutProps {
  children: React.ReactNode
}

export default function AuthLayout({ children }: AuthLayoutProps) {
  return (
    <div className="auth-layout">
      <div className="auth-container">
        <div className="auth-content">{children}</div>
      </div>
    </div>
  )
}
