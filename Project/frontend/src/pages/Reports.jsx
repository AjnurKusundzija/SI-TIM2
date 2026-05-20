import AdminDashboardSection from '../components/admin/AdminDashboardSection'

export default function Reports() {
  return (
    <div className="space-y-6">
      <div className="bg-gradient-to-r from-navy-800 to-navy-700 rounded-xl p-6 text-white">
        <h2 className="text-xl font-bold">Izvještaji</h2>
        <p className="text-navy-200 text-sm mt-1">
          Generisanje izvještaja za odabrani vremenski period
        </p>
      </div>
      <AdminDashboardSection mode="reports" />
    </div>
  )
}
