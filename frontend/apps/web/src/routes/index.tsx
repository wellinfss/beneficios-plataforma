import { createBrowserRouter } from 'react-router-dom'
import { PrivateRoute } from './PrivateRoute'
import LoginPage from '@/pages/LoginPage'
import NotFoundPage from '@/pages/NotFoundPage'
import ForbiddenPage from '@/pages/ForbiddenPage'
import AppLayout from '@/layouts/AppLayout'
import DashboardPage from '@/pages/DashboardPage'
import { ForgotPasswordPage } from '@/pages/ForgotPasswordPage'
import { ResetPasswordPage } from '@/pages/ResetPasswordPage'
import { GruposEconomicosPage } from '@/pages/organizacao/GruposEconomicosPage'
import { GrupoEconomicoFormPage } from '@/pages/organizacao/GrupoEconomicoFormPage'
import { EstipulantesPage } from '@/pages/organizacao/EstipulantesPage'
import { EstipulanteFormPage } from '@/pages/organizacao/EstipulanteFormPage'
import { EstipulanteDetailPage } from '@/pages/organizacao/EstipulanteDetailPage'
import { SubestipulantesPage } from '@/pages/organizacao/SubestipulantesPage'
import { SubestipulanteFormPage } from '@/pages/organizacao/SubestipulanteFormPage'

const router = createBrowserRouter([
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/forgot-password',
    element: <ForgotPasswordPage />,
  },
  {
    path: '/reset-password',
    element: <ResetPasswordPage />,
  },
  {
    path: '/',
    element: (
      <PrivateRoute>
        <AppLayout />
      </PrivateRoute>
    ),
    children: [
      {
        path: 'dashboard',
        element: <DashboardPage />,
      },
      {
        index: true,
        element: <DashboardPage />,
      },
      {
        path: 'organizacao/grupos-economicos',
        element: <GruposEconomicosPage />,
      },
      {
        path: 'organizacao/grupos-economicos/novo',
        element: <GrupoEconomicoFormPage />,
      },
      {
        path: 'organizacao/grupos-economicos/:id',
        element: <GrupoEconomicoFormPage />,
      },
      {
        path: 'organizacao/estipulantes',
        element: <EstipulantesPage />,
      },
      {
        path: 'organizacao/estipulantes/novo',
        element: <EstipulanteFormPage />,
      },
      {
        path: 'organizacao/estipulantes/:id',
        element: <EstipulanteDetailPage />,
      },
      {
        path: 'organizacao/estipulantes/:id/editar',
        element: <EstipulanteFormPage />,
      },
      {
        path: 'organizacao/subestipulantes',
        element: <SubestipulantesPage />,
      },
      {
        path: 'organizacao/subestipulantes/novo',
        element: <SubestipulanteFormPage />,
      },
      {
        path: 'organizacao/subestipulantes/:id/editar',
        element: <SubestipulanteFormPage />,
      },
    ],
  },
  {
    path: '/403',
    element: <ForbiddenPage />,
  },
  {
    path: '*',
    element: <NotFoundPage />,
  },
])

export default router
