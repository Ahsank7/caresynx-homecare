import React, { createContext, useContext, useState, useEffect } from 'react';
import { themeService } from '../services/themeService';

const ThemeContext = createContext();

const applyBodyScheme = (scheme) => {
  if (typeof document === 'undefined') return;
  document.body.classList.toggle('mantine-dark', scheme === 'dark');
};

export const useTheme = () => {
  const context = useContext(ThemeContext);
  if (!context) {
    throw new Error('useTheme must be used within a ThemeProvider');
  }
  return context;
};

export const ThemeProvider = ({ children }) => {
  const [colorScheme, setColorScheme] = useState('light');

  useEffect(() => {
    const savedScheme = themeService.getColorScheme();
    setColorScheme(savedScheme);
    applyBodyScheme(savedScheme);
  }, []);

  const toggleColorScheme = () => {
    const newScheme = themeService.toggleColorScheme();
    setColorScheme(newScheme);
    applyBodyScheme(newScheme);
  };

  const setColorSchemeValue = (scheme) => {
    themeService.setColorScheme(scheme);
    setColorScheme(scheme);
    applyBodyScheme(scheme);
  };

  const value = {
    colorScheme,
    toggleColorScheme,
    setColorScheme: setColorSchemeValue,
  };

  return (
    <ThemeContext.Provider value={value}>
      {children}
    </ThemeContext.Provider>
  );
}; 