import {
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  OnInit,
  Output,
  forwardRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';

import {
  DateConverterService,
  BsDateParts
} from '../date-convertor.service';

type CalendarMode = 'bs' | 'ad';
type CalendarVariant = 'bs' | 'ad' | 'both';

@Component({
  selector: 'app-np-datepicker',
  standalone: true,
  imports: [CommonModule, FormsModule, InputTextModule],
  templateUrl: './np-datepicker.component.html',
  styleUrls: ['./np-datepicker.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => NpDatepickerComponent),
      multi: true
    }
  ]
})
export class NpDatepickerComponent implements ControlValueAccessor, OnInit {
  @Input() placeholder = 'Select Date';
  @Input() minDate?: BsDateParts;
  @Input() maxDate?: BsDateParts;
  @Input() language: 'ne' | 'en' = 'ne';
  @Input() unicodeDate = true;

  @Input() calendarVariant: CalendarVariant = 'both';
  @Input() defaultCalendar: CalendarMode = 'bs';

  @Output() dateChange = new EventEmitter<{ bs: string; ad: string }>();

  showCalendar = false;
  calendarMode: CalendarMode = 'bs';

  selectedBS = '';
  selectedAD = '';
  displayDate = '';

  currentYear!: number;
  currentMonth!: number;

  years: number[] = [];
  adYears: number[] = [];

  months: { bs: string; en: string }[] = [
    { bs: 'बैशाख', en: 'Baisakh' },
    { bs: 'जेठ', en: 'Jestha' },
    { bs: 'असार', en: 'Asar' },
    { bs: 'श्रावण', en: 'Shrawan' },
    { bs: 'भाद्र', en: 'Bhadra' },
    { bs: 'आश्विन', en: 'Ashwin' },
    { bs: 'कार्तिक', en: 'Kartik' },
    { bs: 'मंसिर', en: 'Mangsir' },
    { bs: 'पौष', en: 'Poush' },
    { bs: 'माघ', en: 'Magh' },
    { bs: 'फाल्गुण', en: 'Falgun' },
    { bs: 'चैत्र', en: 'Chaitra' }
  ];

  adMonths = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December'
  ];

  calendar: (number | null)[][] = [];

  private onChange: (value: string | null) => void = () => {};
  private onTouched: () => void = () => {};

  constructor(
    private readonly converter: DateConverterService,
    private readonly elementRef: ElementRef
  ) {}

  ngOnInit(): void {
    this.normalizeCalendarInputs();

    const today = new Date();
    const todayBs = this.converter.adToBsParts(today);

    this.selectedBS = `${todayBs.year}-${this.pad(todayBs.month)}-${this.pad(todayBs.day)}`;
    this.selectedAD = this.converter.formatAd(today);

    this.displayDate = this.formatDisplayValue();

    if (this.calendarMode === 'bs') {
      this.currentYear = todayBs.year;
      this.currentMonth = todayBs.month;
    } else {
      this.currentYear = today.getFullYear();
      this.currentMonth = today.getMonth() + 1;
    }

    this.generateYears();
    this.generateAdYears();
    this.generateCalendar();

    this.onChange(this.selectedBS);
    this.dateChange.emit({ bs: this.selectedBS, ad: this.selectedAD });
  }

  writeValue(obj: any): void {
    if (!obj) {
      return;
    }

    this.selectedBS = obj;
    this.selectedAD = this.converter.bsToAd(obj);
    this.displayDate = this.formatDisplayValue();

    if (this.calendarMode === 'bs') {
      const [y, m] = this.selectedBS.split('-').map(Number);
      this.currentYear = y;
      this.currentMonth = m;
    } else {
      const [y, m] = this.selectedAD.split('-').map(Number);
      this.currentYear = y;
      this.currentMonth = m;
    }

    this.generateCalendar();
  }

  registerOnChange(fn: (value: string | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(_isDisabled: boolean): void {}

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const clickedInside = this.elementRef.nativeElement.contains(event.target);
    if (!clickedInside) {
      this.showCalendar = false;
    }
  }

  toggleCalendar(): void {
    this.showCalendar = !this.showCalendar;
  }

  switchMode(mode: CalendarMode): void {
    if (!this.canUseCalendar(mode)) return;

    this.calendarMode = mode;

    if (mode === 'ad') {
      const [y, m] = this.selectedAD.split('-').map(Number);
      this.currentYear = y;
      this.currentMonth = m;
    } else {
      const [y, m] = this.selectedBS.split('-').map(Number);
      this.currentYear = y;
      this.currentMonth = m;
    }

    this.generateCalendar();
  }

  canUseCalendar(mode: CalendarMode): boolean {
    return this.calendarVariant === 'both' || this.calendarVariant === mode;
  }

  selectYearMonth(year: number, month: number): void {
    this.currentYear = +year;
    this.currentMonth = +month;
    this.generateCalendar();
  }

  selectDate(day: number | null): void {
    if (!day) return;

    if (this.calendarMode === 'bs') {
      const bsString = `${this.currentYear}-${this.pad(this.currentMonth)}-${this.pad(day)}`;
      const adString = this.converter.bsToAd(bsString);

      this.selectedBS = bsString;
      this.selectedAD = adString;
    } else {
      const adString = `${this.currentYear}-${this.pad(this.currentMonth)}-${this.pad(day)}`;
      const bsString = this.converter.adToBs(adString);

      this.selectedAD = adString;
      this.selectedBS = bsString;
    }

    this.displayDate = this.formatDisplayValue();

    this.onChange(this.selectedBS);
    this.onTouched();
    this.dateChange.emit({ bs: this.selectedBS, ad: this.selectedAD });

    this.generateCalendar();
  }

  prevMonth(): void {
    if (this.currentMonth === 1) {
      this.currentYear--;
      this.currentMonth = 12;
    } else {
      this.currentMonth--;
    }
    this.generateCalendar();
  }

  nextMonth(): void {
    if (this.currentMonth === 12) {
      this.currentYear++;
      this.currentMonth = 1;
    } else {
      this.currentMonth++;
    }
    this.generateCalendar();
  }

  selectToday(): void {
    const today = new Date();

    if (this.calendarMode === 'ad') {
      this.currentYear = today.getFullYear();
      this.currentMonth = today.getMonth() + 1;
      this.selectDate(today.getDate());
    } else {
      const bs = this.converter.adToBsParts(today);
      this.currentYear = bs.year;
      this.currentMonth = bs.month;
      this.selectDate(bs.day);
    }
  }

  isSelected(day: number | null): boolean {
    if (!day) return false;

    const current = `${this.currentYear}-${this.pad(this.currentMonth)}-${this.pad(day)}`;

    if (this.calendarMode === 'bs') {
      return this.selectedBS === current;
    }

    return this.selectedAD === current;
  }

  isToday(day: number | null): boolean {
    if (!day) return false;

    const today = new Date();

    if (this.calendarMode === 'ad') {
      const todayAd = this.converter.formatAd(today);
      const current = `${this.currentYear}-${this.pad(this.currentMonth)}-${this.pad(day)}`;
      return todayAd === current;
    }

    const todayBs = this.converter.adToBs(today);
    const current = `${this.currentYear}-${this.pad(this.currentMonth)}-${this.pad(day)}`;
    return todayBs === current;
  }

  isDisabled(day: number): boolean {
    if (!day) return true;

    const adDate =
      this.calendarMode === 'bs'
        ? this.converter.bsToAdParts({
            year: this.currentYear,
            month: this.currentMonth,
            day
          })
        : {
            year: this.currentYear,
            month: this.currentMonth,
            day
          };

    const currentAd = new Date(adDate.year, adDate.month - 1, adDate.day);

    if (this.minDate) {
      const minAd = this.converter.bsToAdParts(this.minDate);
      const minDateObj = new Date(minAd.year, minAd.month - 1, minAd.day);
      if (currentAd < minDateObj) return true;
    }

    if (this.maxDate) {
      const maxAd = this.converter.bsToAdParts(this.maxDate);
      const maxDateObj = new Date(maxAd.year, maxAd.month - 1, maxAd.day);
      if (currentAd > maxDateObj) return true;
    }

    return false;
  }

  generateYears(): void {
    const minY = this.minDate?.year ?? 2070;
    const maxY = this.maxDate?.year ?? 2099;
    this.years = [];
    for (let y = minY; y <= maxY; y++) {
      this.years.push(y);
    }
  }

  generateAdYears(): void {
    this.adYears = [];
    for (let y = 1913; y <= 2045; y++) {
      this.adYears.push(y);
    }
  }

  generateCalendar(): void {
    if (this.calendarMode === 'bs') {
      this.generateBsCalendar();
    } else {
      this.generateAdCalendar();
    }
  }

  private generateBsCalendar(): void {
    try {
      const firstDayAd = this.converter.bsToAdParts({
        year: this.currentYear,
        month: this.currentMonth,
        day: 1
      });

      const firstDayJs = new Date(firstDayAd.year, firstDayAd.month - 1, firstDayAd.day);
      const startDayIndex = firstDayJs.getDay();

      let nextMonth = this.currentMonth + 1;
      let nextYear = this.currentYear;

      if (nextMonth > 12) {
        nextMonth = 1;
        nextYear++;
      }

      let daysInMonth = 30;

      try {
        const nextMonthAd = this.converter.bsToAdParts({
          year: nextYear,
          month: nextMonth,
          day: 1
        });

        const nextMonthJs = new Date(nextMonthAd.year, nextMonthAd.month - 1, nextMonthAd.day);

        const diffDays = Math.round(
          (nextMonthJs.getTime() - firstDayJs.getTime()) / (1000 * 60 * 60 * 24)
        );

        if (diffDays >= 28 && diffDays <= 32) {
          daysInMonth = diffDays;
        }
      } catch {
        daysInMonth = 30;
      }

      this.calendar = this.buildCalendarMatrix(startDayIndex, daysInMonth);
    } catch {
      this.calendar = [];
    }
  }

  private generateAdCalendar(): void {
    const firstDayJs = new Date(this.currentYear, this.currentMonth - 1, 1);
    const startDayIndex = firstDayJs.getDay();
    const daysInMonth = new Date(this.currentYear, this.currentMonth, 0).getDate();

    this.calendar = this.buildCalendarMatrix(startDayIndex, daysInMonth);
  }

  private buildCalendarMatrix(startDayIndex: number, daysInMonth: number): (number | null)[][] {
    const weeks: (number | null)[][] = [];
    let week: (number | null)[] = [];

    for (let i = 0; i < startDayIndex; i++) {
      week.push(null);
    }

    for (let d = 1; d <= daysInMonth; d++) {
      week.push(d);

      if (week.length === 7) {
        weeks.push(week);
        week = [];
      }
    }

    if (week.length) {
      while (week.length < 7) {
        week.push(null);
      }
      weeks.push(week);
    }

    return weeks;
  }

  toNepaliDigits(str: string): string {
    const digits = ['०', '१', '२', '३', '४', '५', '६', '७', '८', '९'];
    return str.replace(/\d/g, d => digits[parseInt(d, 10)]);
  }

  private normalizeCalendarInputs(): void {
    if (this.calendarVariant === 'bs') {
      this.calendarMode = 'bs';
      return;
    }

    if (this.calendarVariant === 'ad') {
      this.calendarMode = 'ad';
      return;
    }

    this.calendarMode = this.defaultCalendar;
  }

  private formatDisplayValue(): string {
    if (this.calendarMode === 'ad' && this.calendarVariant === 'ad') {
      return this.selectedAD;
    }

    return this.unicodeDate ? this.toNepaliDigits(this.selectedBS) : this.selectedBS;
  }

  private pad(value: number): string {
    return String(value).padStart(2, '0');
  }
}