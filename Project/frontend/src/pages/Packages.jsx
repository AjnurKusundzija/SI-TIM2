import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
    Package as PackageIcon,
    Wifi,
    Tv,
    Smartphone,
    Layers,
    Calendar,
    Wallet,
    ChevronRight,
} from 'lucide-react'
import { getMyPackages } from '../services/packageService'
import EmptyState from '../components/common/EmptyState'
import Badge from '../components/common/Badge'

const TYPE_LABELS = {
    INTERNET: 'Internet',
    TV: 'TV',
    MOBILE: 'Mobilni',
    BUNDLE: 'Kombinovani',
}

const TYPE_ICONS = {
    INTERNET: Wifi,
    TV: Tv,
    MOBILE: Smartphone,
    BUNDLE: Layers,
}

function formatPrice(price) {
    if (price === null || price === undefined) return ''
    const num = Number(price)
    if (Number.isNaN(num)) return ''
    return `${num.toFixed(2).replace('.', ',')} KM / mjesec`
}

function formatDate(value) {
    if (!value) return null
    const d = new Date(value)
    if (Number.isNaN(d.getTime())) return null
    return d.toLocaleDateString('hr-HR')
}

function PackagesSkeleton() {
    return (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {Array.from({ length: 6 }).map((_, i) => (
                <div
                    key={i}
                    className="bg-white rounded-xl shadow-sm border border-gray-100 p-5 animate-pulse"
                >
                    <div className="flex items-center justify-between mb-3">
                        <div className="h-10 w-10 rounded-lg bg-gray-100" />
                        <div className="h-5 w-16 bg-gray-100 rounded-full" />
                    </div>
                    <div className="h-4 w-3/4 bg-gray-200 rounded mb-2" />
                    <div className="h-3 w-1/2 bg-gray-100 rounded mb-4" />
                    <div className="h-3 w-full bg-gray-100 rounded mb-1.5" />
                    <div className="h-3 w-5/6 bg-gray-100 rounded" />
                </div>
            ))}
        </div>
    )
}

export default function Packages() {
    const navigate = useNavigate()
    const [packages, setPackages] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)

    useEffect(() => {
        getMyPackages()
            .then(setPackages)
            .catch((err) => {
                console.error(err)
                setError('Greška pri učitavanju paketa.')
            })
            .finally(() => setLoading(false))
    }, [])

    return (
        <div className="space-y-5">
            <div>
                <h1 className="text-xl font-semibold text-gray-900">Moji paketi</h1>
                <p className="text-sm text-gray-500 mt-1">
                    Pregled vaših aktivnih paketa i pretplata.
                </p>
            </div>

            {loading ? (
                <PackagesSkeleton />
            ) : error ? (
                <div className="p-4 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">
                    {error}
                </div>
            ) : packages.length === 0 ? (
                <EmptyState
                    icon={PackageIcon}
                    title="Nemate aktivnih paketa ili pretplata."
                    description="Trenutno nemate aktivnih paketa povezanih sa vašim računom."
                />
            ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                    {packages.map((pkg) => {
                        const Icon = TYPE_ICONS[pkg.packageType] ?? PackageIcon
                        const startDate = formatDate(pkg.startDate)
                        return (
                            <button
                                key={pkg.packageId}
                                onClick={() => navigate(`/packages/${pkg.packageId}`)}
                                className="text-left bg-white rounded-xl shadow-sm border border-gray-100 p-5 flex flex-col hover:shadow-md hover:border-navy-200 transition-all group"
                            >
                                <div className="flex items-start justify-between mb-3">
                                    <div className="h-10 w-10 rounded-lg bg-navy-50 flex items-center justify-center text-navy-700">
                                        <Icon size={20} />
                                    </div>
                                    <Badge value={pkg.packageStatus} />
                                </div>

                                <h3 className="text-base font-semibold text-gray-900 mb-1 line-clamp-2">
                                    {pkg.packageName}
                                </h3>

                                <div className="flex items-center gap-2 mb-3">
                                    <Badge value={pkg.packageType} />
                                </div>

                                <p className="text-sm text-gray-600 mb-3 line-clamp-2">
                                    {pkg.packageDescription || pkg.summary}
                                </p>

                                {pkg.includedServices?.length > 0 && (
                                    <div className="flex flex-wrap gap-1.5 mb-4">
                                        {pkg.includedServices.map((svc) => (
                                            <span
                                                key={svc}
                                                className="inline-flex items-center px-2 py-0.5 rounded-md text-xs font-medium bg-gray-100 text-gray-700"
                                            >
                                                {svc}
                                            </span>
                                        ))}
                                    </div>
                                )}

                                <div className="mt-auto pt-3 border-t border-gray-100 space-y-1.5">
                                    <div className="flex items-center justify-between">
                                        <div className="flex items-center gap-1.5 text-sm font-medium text-gray-900">
                                            <Wallet size={14} className="text-gray-400" />
                                            {formatPrice(pkg.monthlyPrice)}
                                        </div>
                                        <span className="inline-flex items-center gap-1 text-xs font-medium text-navy-700 group-hover:text-navy-900">
                                            Detalji
                                            <ChevronRight size={14} />
                                        </span>
                                    </div>
                                    {startDate && (
                                        <div className="flex items-center gap-1.5 text-xs text-gray-500">
                                            <Calendar size={12} />
                                            Početak pretplate: <strong className="text-gray-700 font-medium">{startDate}</strong>
                                        </div>
                                    )}
                                </div>
                            </button>
                        )
                    })}
                </div>
            )}
        </div>
    )
}

export { TYPE_LABELS, TYPE_ICONS }
