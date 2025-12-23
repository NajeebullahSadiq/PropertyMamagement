export enum CalendarType {
  GREGORIAN = 'Gregorian',
  HIJRI_SHAMSI = 'HijriShamsi',
  HIJRI_QAMARI = 'HijriQamari'
}

export interface CalendarOption {
  value: CalendarType;
  label: string;
  labelFa: string;
}

export const CALENDAR_OPTIONS: CalendarOption[] = [
  {
    value: CalendarType.HIJRI_SHAMSI,
    label: 'Hijri Shamsi (Afghan/Persian)',
    labelFa: 'هجری شمسی'
  },
  {
    value: CalendarType.HIJRI_QAMARI,
    label: 'Hijri Qamari (Islamic)',
    labelFa: 'هجری قمری'
  },
  {
    value: CalendarType.GREGORIAN,
    label: 'Gregorian',
    labelFa: 'میلادی'
  }
];
