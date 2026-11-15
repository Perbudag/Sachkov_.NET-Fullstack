import { z } from "zod";

const requiredText = (label: string) =>
  z.string().trim().min(1, `Поле "${label}" обязательно для заполнения`);

const postalCodePattern = /^[0-9|A-Z| |-]{4,8}$/;
const namePattern = /^[а-я|А-Я|a-z|A-Z| ]+$/;
const numberPattern = /^[0-9|а-я|a-z]+$/;

export const createLocationSchema = z.object({
  name: requiredText("Название")
    .min(2, 'Поле "Название" должно содержать не менее 2 символов')
    .max(150, 'Поле "Название" должно содержать не более 150 символов'),
  address: z.object({
    postalCode: requiredText("Почтовый индекс").regex(
      postalCodePattern,
      "Почтовый индекс должен содержать от 4 до 8 символов: цифры, заглавные латинские буквы, пробелы или дефисы",
    ),
    country: requiredText("Страна").regex(
      namePattern,
      "Страна должна содержать только буквы и пробелы",
    ),
    region: requiredText("Регион").regex(
      namePattern,
      "Регион должен содержать только буквы и пробелы",
    ),
    city: requiredText("Город").regex(
      namePattern,
      "Город должен содержать только буквы и пробелы",
    ),
    street: requiredText("Улица").regex(
      namePattern,
      "Улица должна содержать только буквы и пробелы",
    ),
    house: requiredText("Номер дома").regex(
      numberPattern,
      "Номер дома должен содержать только цифры или строчные буквы",
    ),
    apartment: z
      .string()
      .trim()
      .refine(
        (value) => !value || numberPattern.test(value),
        "Номер квартиры должен содержать только цифры или строчные буквы",
      )
      .transform((value) => value || null)
      .nullable(),
  }),
});

export type CreateLocationFormValues = z.input<typeof createLocationSchema>;
export type CreateLocationSubmission = z.output<typeof createLocationSchema>;
