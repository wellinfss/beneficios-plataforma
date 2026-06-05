import { useAuthStore } from '@/store/authStore'

export default function DashboardPage() {
  const { user } = useAuthStore()

  return (
    <div className="dashboard-container">
      <h1>Dashboard</h1>
      <p>Welcome, {user?.name || 'User'}!</p>
      <div className="placeholder-content">
        <p>Dashboard content will be added here.</p>
      </div>
    </div>
  )
}
