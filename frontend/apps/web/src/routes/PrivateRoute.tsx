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

  // TODO: Check user permissions from token or store
  if (!user) {
    return fallback
  }

  return <>{children}</>
}
