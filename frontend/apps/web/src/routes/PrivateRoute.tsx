import { Navigate } from 'react-router-dom'
import { useAuthStore } from '@/store/authStore'

interface PrivateRouteProps {
  children: React.ReactNode
}

export function PrivateRoute({ children }: PrivateRouteProps) {
  const { isAuthenticated } = useAuthStore()

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />
  }

  return <>{children}</>
}

interface PermissionGuardProps {
  permission: string
  children: React.ReactNode
  fallback?: React.ReactNode
}

export function PermissionGuard({
  permission,
  children,
  fallback = <Navigate to="/403" replace />,
}: PermissionGuardProps) {
  const { user } = useAuthStore()

  if (!user) {
    return fallback
  }

  if (user.permissions && user.permissions.includes(permission)) {
    return <>{children}</>
  }

  return fallback
}
