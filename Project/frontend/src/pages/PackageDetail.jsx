import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import {
    ArrowLeft,
    Wifi,
    Tv,
    Smartphone,
    Layers,
    Package as PackageIcon,
    Calendar,
    Info,
} from 'lucide-react'
import { getPackageById } from '../services/packageService'
import Badge from '../components/common/Badge'
import { formatDateOnly } from '../utils/formatDate'

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

function FeatureRow({ feature }) {
    const hasUnit = feature.unit && feature.unit.trim() !== ''
    return (
        <div className="flex items-start justify-between gap-4 py-3 border-b border-gray-100 last:border-0">
            <div className="min-w-0">
                <p className="text-sm font-medium text-gray-900">{feature.name}</p>
                {feature.description && (
                    <p className="text-xs text-gray-500 mt-0.5">{feature.description}</p>
                )}
            </div>
            <p className="text-sm font-semibold text-gray-900 whitespace-nowrap">
                {feature.value}
                {hasUnit && <span className="text-gray-500 font-normal"> {feature.unit}</span>}
            </p>
        </div>
    )
}

function DetailSkeleton() {
    return (
        <div className="max-w-4xl mx-auto space-y-5 animate-pulse">
            <div className="h-4 w-32 bg-gray-200 rounded" />
            <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 space-y-4">
                <div className="flex items-start justify-between">
                    <div className="space-y-2">
                        <div className="h-6 w-64 bg-gray-200 rounded" />
                        <div className="h-3 w-24 bg-gray-100 rounded" />
                    </div>
                    <div className="h-6 w-20 bg-gray-100 rounded-full" />
                </div>
                <div className="h-3 w-full bg-gray-100 rounded" />
                <div className="h-3 w-5/6 bg-gray-100 rounded" />
            </div>
            <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 space-y-3">
                {[0, 1, 2, 3].map((i) => (
                    <div key={i} className="h-4 bg-gray-100 rounded" />
                ))}
            </div>
        </div>
    )
}

export default function PackageDetail() {
    const navigate = useNavigate()
    const { id } = useParams()
    const [pkg, setPkg] = useState(null)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)

    useEffect(() => {
        let cancelled = false

        getPackageById(id)
            .then((data) => {
                if (!cancelled) setPkg(data)
            })
            .catch((err) => {
                if (cancelled) return
                console.error(err)
                if (err.response?.status === 403) {
                    setError('Nemate pristup ovom paketu.')
                } else if (err.response?.status === 404) {
                    setError('Traženi paket nije pronađen.')
                } else {
                    setError('Greška pri učitavanju paketa.')
                }
            })
            .finally(() => {
                if (!cancelled) setLoading(false)
            })

        return () => {
            cancelled = true
        }
    }, [id])

    if (loading) return <DetailSkeleton />

    if (error) {
        return (
            <div className="max-w-4xl mx-auto space-y-4">
                <Link
                    to="/packages"
                    className="inline-flex items-center gap-1 text-sm text-navy-700 hover:text-navy-900"
                >
                    <ArrowLeft size={14} />
                    Nazad na pakete
                </Link>
                <div className="p-4 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">
                    {error}
                </div>
            </div>
        )
    }

    if (!pkg) return null

    const Icon = TYPE_ICONS[pkg.packageType] ?? PackageIcon

    return (
        <div className="max-w-4xl mx-auto space-y-5">
            <button
                onClick={() => navigate('/packages')}
                className="inline-flex items-center gap-1 text-sm text-navy-700 hover:text-navy-900"
            >
                <ArrowLeft size={14} />
                Nazad na pakete
            </button>

            {/* Header section */}
            <section className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
                <div className="flex flex-col lg:flex-row lg:items-start lg:justify-between gap-4">
                    <div className="flex items-start gap-4">
                        <div className="h-14 w-14 rounded-xl bg-navy-50 flex items-center justify-center text-navy-700 shrink-0">
                            <Icon size={28} />
                        </div>
                        <div>
                            <h1 className="text-xl font-semibold text-gray-900">
                                {pkg.packageName}
                            </h1>
                            <div className="flex flex-wrap items-center gap-2 mt-2">
                                <Badge value={pkg.packageType} />
                                <Badge value={pkg.packageStatus} />
                            </div>
                        </div>
                    </div>

                    <div className="text-right shrink-0">
                        <p className="text-xs text-gray-500 uppercase tracking-wide">
                            Mjesečna pretplata
                        </p>
                        <p className="text-lg font-semibold text-gray-900 mt-0.5">
                            {formatPrice(pkg.monthlyPrice)}
                        </p>
                    </div>
                </div>

                {pkg.packageDescription && (
                    <div className="mt-5 pt-5 border-t border-gray-100">
                        <div className="flex items-start gap-2 text-sm text-gray-600">
                            <Info size={14} className="text-gray-400 mt-0.5 shrink-0" />
                            <p>{pkg.packageDescription}</p>
                        </div>
                    </div>
                )}

                {(pkg.startDate || pkg.endDate) && (
                    <div className="mt-4 pt-4 border-t border-gray-100 grid grid-cols-1 sm:grid-cols-2 gap-4">
                        {pkg.startDate && (
                            <div className="flex items-center gap-2 text-sm">
                                <Calendar size={14} className="text-gray-400" />
                                <span className="text-gray-500">Početak pretplate:</span>
                                <span className="text-gray-900 font-medium">
                                    {formatDateOnly(pkg.startDate)}
                                </span>
                            </div>
                        )}
                        {pkg.endDate && (
                            <div className="flex items-center gap-2 text-sm">
                                <Calendar size={14} className="text-gray-400" />
                                <span className="text-gray-500">Datum isteka:</span>
                                <span className="text-gray-900 font-medium">
                                    {formatDateOnly(pkg.endDate)}
                                </span>
                            </div>
                        )}
                    </div>
                )}
            </section>

            {/* Features section */}
            <section className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
                <h2 className="text-sm font-semibold text-gray-900 uppercase tracking-wide mb-3">
                    Uključene usluge
                </h2>

                {pkg.features?.length > 0 ? (
                    <div>
                        {pkg.features.map((f) => (
                            <FeatureRow key={f.featureId} feature={f} />
                        ))}
                    </div>
                ) : (
                    <p className="text-sm text-gray-500">
                        Za ovaj paket trenutno nema dodatnih detalja o uslugama.
                    </p>
                )}
            </section>
        </div>
    )
}
