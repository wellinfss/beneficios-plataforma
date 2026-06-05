import { Navigate } from 'react-router-dom'
import { useAuthStore } from '@/store/authStore'

interface RoleGuardProps {
  role: string
  children: React.ReactNode
  fallback?: React.ReactNode
}

export function RoleGuard({
  role,
  children,
  fallback = <Navigate to="/403" replace />,
}: RoleGuardProps) {
  const { user } = useAuthStore()

  if (!user) {
    return fallback
  }

  if (user.roles && user.roles.includes(role)) {
    return <>{children}</>
  }

  return fallback
}
