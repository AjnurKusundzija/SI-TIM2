import PropTypes from 'prop-types'

function formatRemaining(minutes) {
  if (minutes == null) return null
  if (minutes <= 0) {
    const over = Math.abs(minutes)
    if (over < 60) return `Prekoračeno ${Math.round(over)} min`
    return `Prekoračeno ${Math.round(over / 60)}h`
  }
  if (minutes < 60) return `${Math.round(minutes)} min`
  const h = Math.floor(minutes / 60)
  const m = Math.round(minutes % 60)
  return m > 0 ? `${h}h ${m}m` : `${h}h`
}

const COLOR = {
  GREEN:  'bg-green-100 text-green-800 border-green-200',
  YELLOW: 'bg-yellow-100 text-yellow-800 border-yellow-200',
  RED:    'bg-red-100 text-red-800 border-red-200',
}

const DOT = {
  GREEN:  'bg-green-500',
  YELLOW: 'bg-yellow-500',
  RED:    'bg-red-500',
}

export default function SlaIndicator({ slaStatus, slaRemainingMinutes, slaIsBreached, compact = false }) {
  if (!slaStatus) return <span className="text-xs text-gray-400 italic">—</span>

  const status = slaStatus ?? 'GREEN'
  const label = formatRemaining(slaRemainingMinutes)

  if (compact) {
    return (
      <span
        title={label ?? undefined}
        className={`inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-[11px] font-medium border ${COLOR[status] ?? COLOR.RED}`}
      >
        <span className={`w-1.5 h-1.5 rounded-full flex-shrink-0 ${DOT[status] ?? DOT.RED}`} />
        {slaIsBreached ? 'Prekoračeno' : label}
      </span>
    )
  }

  return (
    <div className={`inline-flex flex-col items-start gap-0.5 px-3 py-2 rounded-lg border ${COLOR[status] ?? COLOR.RED}`}>
      <div className="flex items-center gap-1.5">
        <span className={`w-2 h-2 rounded-full flex-shrink-0 ${DOT[status] ?? DOT.RED}`} />
        <span className="text-xs font-semibold">
          {slaIsBreached ? 'SLA prekoračen' : 'SLA ističe za'}
        </span>
      </div>
      {!slaIsBreached && label && (
        <span className="text-sm font-bold pl-3.5">{label}</span>
      )}
    </div>
  )
}

SlaIndicator.propTypes = {
  slaStatus: PropTypes.string,
  slaRemainingMinutes: PropTypes.number,
  slaIsBreached: PropTypes.bool,
  compact: PropTypes.bool,
}
