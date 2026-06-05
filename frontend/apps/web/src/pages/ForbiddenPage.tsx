import { useNavigate } from 'react-router-dom'

export default function ForbiddenPage() {
  const navigate = useNavigate()

  return (
    <div className="error-page">
      <h1>403 - Access Forbidden</h1>
      <p>You do not have permission to access this resource.</p>
      <button onClick={() => navigate('/')}>Go to Home</button>
    </div>
  )
}
