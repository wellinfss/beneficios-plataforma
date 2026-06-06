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
import { OperadorasPage } from '@/pages/catalogo/OperadorasPage'
import { OperadoraFormPage } from '@/pages/catalogo/OperadoraFormPage'
import { OperadoraDetailPage } from '@/pages/catalogo/OperadoraDetailPage'
import { ProdutosPage } from '@/pages/catalogo/ProdutosPage'
import { ProdutoFormPage } from '@/pages/catalogo/ProdutoFormPage'
import { ProdutoDetailPage } from '@/pages/catalogo/ProdutoDetailPage'
import { PlanosPage } from '@/pages/catalogo/PlanosPage'
import { PlanoFormPage } from '@/pages/catalogo/PlanoFormPage'

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
      {
        path: 'catalogo/operadoras',
        element: <OperadorasPage />,
      },
      {
        path: 'catalogo/operadoras/novo',
        element: <OperadoraFormPage />,
      },
      {
        path: 'catalogo/operadoras/:id',
        element: <OperadoraDetailPage />,
      },
      {
        path: 'catalogo/operadoras/:id/editar',
        element: <OperadoraFormPage />,
      },
      {
        path: 'catalogo/produtos',
        element: <ProdutosPage />,
      },
      {
        path: 'catalogo/produtos/novo',
        element: <ProdutoFormPage />,
      },
      {
        path: 'catalogo/produtos/:id',
        element: <ProdutoDetailPage />,
      },
      {
        path: 'catalogo/produtos/:id/editar',
        element: <ProdutoFormPage />,
      },
      {
        path: 'catalogo/planos',
        element: <PlanosPage />,
      },
      {
        path: 'catalogo/planos/novo',
        element: <PlanoFormPage />,
      },
      {
        path: 'catalogo/planos/:id/editar',
        element: <PlanoFormPage />,
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
