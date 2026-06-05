import { Outlet, useNavigate } from 'react-router-dom'
import { useAuthStore } from '@/store/authStore'

export default function AppLayout() {
  const navigate = useNavigate()
  const { user, logout } = useAuthStore()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  return (
    <div className="app-layout">
      <aside className="sidebar">
        <div className="sidebar-header">
          <h2>Menu</h2>
        </div>
        <nav className="sidebar-nav">
          <ul>
            <li>
              <a href="/">Dashboard</a>
            </li>
            <li>
              <a href="/beneficiarios">Beneficiários</a>
            </li>
            <li>
              <a href="/contratos">Contratos</a>
            </li>
            <li>
              <a href="/operadoras">Operadoras</a>
            </li>
            <li>
              <a href="/auditoria">Auditoria</a>
            </li>
          </ul>
        </nav>
      </aside>

      <div className="app-container">
        <header className="app-header">
          <div className="header-content">
            <h1>Benefícios Plataforma</h1>
            <div className="user-menu">
              <span>{user?.name || 'User'}</span>
              <button onClick={handleLogout}>Logout</button>
            </div>
          </div>
        </header>

        <main className="app-content">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
