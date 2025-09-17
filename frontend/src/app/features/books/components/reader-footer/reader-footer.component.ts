import { Component, HostListener, input, output } from '@angular/core';
import { ChevronLeft, ChevronRight, LucideAngularModule, Settings } from 'lucide-angular';
import { ReaderSettingsMenuComponent } from './setting-panel/setting-panel.component';
import { ReadingSettings } from '../../interfaces/reading-setting.interface';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'reader-footer',
  imports: [LucideAngularModule, ReaderSettingsMenuComponent, RouterLink],
  templateUrl: './reader-footer.component.html',
})
export class ReaderFooterComponent {
  bookId = input<number|null>();
  previousChapter = input<number|null>();
  nextChapter = input<number|null>();

  showFooter = true;  // controla si el footer se muestra o no
  private lastScrollTop = 0;  // guarda la última posición del scroll
  isSettingsOpen = false;  // controla si el menú de configuración está abierto

  @HostListener('window:scroll', [])
  onWindowScroll() {
    // pageYOffset es la posición actual del scroll vertical.
    const st = window.pageYOffset || document.documentElement.scrollTop;
    this.showFooter = st < this.lastScrollTop; // si scrolleás hacia arriba, mostrar footer
    this.lastScrollTop = st <= 0 ? 0 : st; // actualiza la última posición
  }

  // Iconos
  readonly setting = Settings;
  readonly left = ChevronLeft;
  readonly right = ChevronRight;

  // Métodos de navegación
  goToPrev() {
    console.log('Ir al capítulo anterior');
    // Aquí implementarías la lógica para ir al capítulo anterior
  }

  goToNext() {
    console.log('Ir al capítulo siguiente');
    // Aquí implementarías la lógica para ir al capítulo siguiente
  }

  // Métodos del menú de configuración
  toggleSettings() {
    this.isSettingsOpen = !this.isSettingsOpen;
  }

  closeSettings() {
    this.isSettingsOpen = false;
  }

 // Output para comunicar con el parent (reader-page)
  settingsChange = output<ReadingSettings>();

  // Maneja los cambios del settings-menu
  onSettingsChange(settings: ReadingSettings) {
    // Re-emite los eventos al componente padre
    this.settingsChange.emit(settings);
  }
}
