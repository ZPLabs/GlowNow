/**
 * Validates an Ecuador RUC (Registro Único de Contribuyentes).
 * RUC is a 13-digit number where:
 * - First 10 digits are the cedula (personal ID)
 * - Last 3 digits are "001" for businesses
 *
 * For businesses (sociedad), the third digit must be 9.
 * For natural persons (persona natural), the third digit must be 0-5.
 */
export function validateEcuadorRuc(ruc: string): { valid: boolean; error?: string } {
  // Remove any spaces or dashes
  const cleanRuc = ruc.replace(/[\s-]/g, "");

  // Check length
  if (cleanRuc.length !== 13) {
    return { valid: false, error: "RUC must be 13 digits" };
  }

  // Check if all digits
  if (!/^\d{13}$/.test(cleanRuc)) {
    return { valid: false, error: "RUC must contain only numbers" };
  }

  // First two digits are the province code (01-24 or 30)
  const provinceCode = parseInt(cleanRuc.substring(0, 2), 10);
  if ((provinceCode < 1 || provinceCode > 24) && provinceCode !== 30) {
    return { valid: false, error: "Invalid province code" };
  }

  // Third digit determines the type
  const thirdDigit = parseInt(cleanRuc.charAt(2), 10);

  // Last 3 digits must be "001" for businesses
  const lastThree = cleanRuc.substring(10, 13);
  if (lastThree !== "001") {
    return { valid: false, error: "RUC must end in 001" };
  }

  // Validate check digit based on type
  if (thirdDigit === 9) {
    // Private company (Sociedad Privada)
    return validatePrivateCompany(cleanRuc);
  } else if (thirdDigit === 6) {
    // Public company
    return validatePublicCompany(cleanRuc);
  } else if (thirdDigit >= 0 && thirdDigit <= 5) {
    // Natural person
    return validateNaturalPerson(cleanRuc);
  }

  return { valid: false, error: "Invalid RUC type" };
}

function validateNaturalPerson(ruc: string): { valid: boolean; error?: string } {
  const cedula = ruc.substring(0, 10);
  const coefficients = [2, 1, 2, 1, 2, 1, 2, 1, 2];
  let sum = 0;

  for (let i = 0; i < 9; i++) {
    const char = cedula.charAt(i);
    const coeff = coefficients[i];
    if (coeff === undefined) continue;
    let digit = parseInt(char, 10) * coeff;
    if (digit >= 10) {
      digit -= 9;
    }
    sum += digit;
  }

  const checkDigit = (10 - (sum % 10)) % 10;
  const actualCheckDigit = parseInt(cedula.charAt(9), 10);

  if (checkDigit !== actualCheckDigit) {
    return { valid: false, error: "Invalid check digit" };
  }

  return { valid: true };
}

function validatePrivateCompany(ruc: string): { valid: boolean; error?: string } {
  const coefficients = [4, 3, 2, 7, 6, 5, 4, 3, 2];
  let sum = 0;

  for (let i = 0; i < 9; i++) {
    const char = ruc.charAt(i);
    const coeff = coefficients[i];
    if (coeff === undefined) continue;
    sum += parseInt(char, 10) * coeff;
  }

  const checkDigit = 11 - (sum % 11);
  const actualCheckDigit = parseInt(ruc.charAt(9), 10);
  const finalCheckDigit = checkDigit === 11 ? 0 : checkDigit;

  if (finalCheckDigit !== actualCheckDigit) {
    return { valid: false, error: "Invalid check digit" };
  }

  return { valid: true };
}

function validatePublicCompany(ruc: string): { valid: boolean; error?: string } {
  const coefficients = [3, 2, 7, 6, 5, 4, 3, 2];
  let sum = 0;

  for (let i = 0; i < 8; i++) {
    const char = ruc.charAt(i);
    const coeff = coefficients[i];
    if (coeff === undefined) continue;
    sum += parseInt(char, 10) * coeff;
  }

  const checkDigit = 11 - (sum % 11);
  const actualCheckDigit = parseInt(ruc.charAt(8), 10);
  const finalCheckDigit = checkDigit === 11 ? 0 : checkDigit;

  if (finalCheckDigit !== actualCheckDigit) {
    return { valid: false, error: "Invalid check digit" };
  }

  return { valid: true };
}

export function rucValidationRule(
  message = "Please enter a valid RUC"
): (value: string) => string | undefined {
  return (value) => {
    if (!value) return undefined;
    const result = validateEcuadorRuc(value);
    if (!result.valid) {
      return message;
    }
    return undefined;
  };
}
