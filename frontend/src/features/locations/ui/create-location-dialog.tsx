"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import {
  createLocationSchema,
  type CreateLocationFormValues,
  type CreateLocationSubmission,
} from "../create-location-schema";
import { Button } from "@/shared/components/ui/button";
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/shared/components/ui/dialog";
import {
  Field,
  FieldDescription,
  FieldError,
  FieldGroup,
  FieldLegend,
  FieldSeparator,
  FieldSet,
} from "@/shared/components/ui/field";
import { Input } from "@/shared/components/ui/input";
import { Label } from "@/shared/components/ui/label";
import React from "react";
import { useCreateLocation } from "../use-create-location";
import { isEnvelopeError } from "@/shared/api/axios-instance";
import { mapCreateLocationErrorsToForm } from "../map-create-location-errors";
import { toast } from "sonner";

const defaultValues: CreateLocationFormValues = {
  name: "",
  address: {
    postalCode: "",
    country: "",
    region: "",
    city: "",
    street: "",
    house: "",
    apartment: "",
  },
};

type Props = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
};

export function CreateLocationDialog({ open, onOpenChange }: Props) {
  const { createLocation, isPending } = useCreateLocation();
  const form = useForm<
    CreateLocationFormValues,
    unknown,
    CreateLocationSubmission
  >({
    resolver: zodResolver(createLocationSchema),
    defaultValues,
  });

  const onSubmit = (values: CreateLocationSubmission) => {
    createLocation(values, {
      onSuccess: () => {
        onOpenChange(false);
        form.reset(defaultValues);
      },
      onError: (error) => {
        if (isEnvelopeError(error)) {
          const unhandledErrors = mapCreateLocationErrorsToForm(
            error.apiErrors,
            form.setError,
          );
          unhandledErrors.forEach((apiError) => toast.error(apiError.message));
        } else {
          toast.error(error.message);
        }
      },
    });
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-2xl" showCloseButton={false}>
        <form noValidate onSubmit={form.handleSubmit(onSubmit)}>
          <DialogHeader>
            <DialogTitle>Создать новую локацию</DialogTitle>
            <DialogDescription>
              Заполните форму для создания новой локации
            </DialogDescription>
          </DialogHeader>
          <FieldGroup>
            <Field>
              <Label htmlFor="location-name">Название</Label>
              <Input
                id="location-name"
                placeholder="Введите название"
                aria-invalid={Boolean(form.formState.errors.name)}
                aria-describedby="location-name-error"
                {...form.register("name")}
              />
              <FieldError id="location-name-error">
                {form.formState.errors.name?.message}
              </FieldError>
            </Field>
          </FieldGroup>
          <FieldSeparator />
          <FieldSet>
            <FieldLegend>Адрес</FieldLegend>
            <FieldDescription>
              Укажите адрес локации, включая почтовый индекс, страну, регион,
              город, улицу и номер дома.
            </FieldDescription>
            <FieldGroup className="grid grid-cols-1 gap-4 sm:grid-cols-2 mb-5">
              <Field>
                <Label htmlFor="location-postalCode">Почтовый индекс</Label>
                <Input
                  id="location-postalCode"
                  placeholder="Введите почтовый индекс"
                  aria-invalid={Boolean(
                    form.formState.errors.address?.postalCode,
                  )}
                  aria-describedby="location-postalCode-error"
                  {...form.register("address.postalCode")}
                />
                <FieldError id="location-postalCode-error">
                  {form.formState.errors.address?.postalCode?.message}
                </FieldError>
              </Field>

              <Field>
                <Label htmlFor="location-country">Страна</Label>
                <Input
                  id="location-country"
                  placeholder="Введите страну"
                  aria-invalid={Boolean(form.formState.errors.address?.country)}
                  aria-describedby="location-country-error"
                  {...form.register("address.country")}
                />
                <FieldError id="location-country-error">
                  {form.formState.errors.address?.country?.message}
                </FieldError>
              </Field>

              <Field>
                <Label htmlFor="location-region">Регион</Label>
                <Input
                  id="location-region"
                  placeholder="Введите регион"
                  aria-invalid={Boolean(form.formState.errors.address?.region)}
                  aria-describedby="location-region-error"
                  {...form.register("address.region")}
                />
                <FieldError id="location-region-error">
                  {form.formState.errors.address?.region?.message}
                </FieldError>
              </Field>

              <Field>
                <Label htmlFor="location-city">Город</Label>
                <Input
                  id="location-city"
                  placeholder="Введите город"
                  aria-invalid={Boolean(form.formState.errors.address?.city)}
                  aria-describedby="location-city-error"
                  {...form.register("address.city")}
                />
                <FieldError id="location-city-error">
                  {form.formState.errors.address?.city?.message}
                </FieldError>
              </Field>

              <Field>
                <Label htmlFor="location-street">Улица</Label>
                <Input
                  id="location-street"
                  placeholder="Введите улицу"
                  aria-invalid={Boolean(form.formState.errors.address?.street)}
                  aria-describedby="location-street-error"
                  {...form.register("address.street")}
                />
                <FieldError id="location-street-error">
                  {form.formState.errors.address?.street?.message}
                </FieldError>
              </Field>

              <Field>
                <Label htmlFor="location-house">Дом</Label>
                <Input
                  id="location-house"
                  placeholder="Введите номер дома"
                  aria-invalid={Boolean(form.formState.errors.address?.house)}
                  aria-describedby="location-house-error"
                  {...form.register("address.house")}
                />
                <FieldError id="location-house-error">
                  {form.formState.errors.address?.house?.message}
                </FieldError>
              </Field>

              <Field>
                <Label htmlFor="location-apartment">Квартира</Label>
                <Input
                  id="location-apartment"
                  placeholder="Введите номер квартиры"
                  aria-invalid={Boolean(
                    form.formState.errors.address?.apartment,
                  )}
                  aria-describedby="location-apartment-error"
                  {...form.register("address.apartment")}
                />
                <FieldError id="location-apartment-error">
                  {form.formState.errors.address?.apartment?.message}
                </FieldError>
              </Field>
            </FieldGroup>
          </FieldSet>
          <DialogFooter>
            <DialogClose
              render={
                <Button variant="outline" disabled={isPending}>
                  Отмена
                </Button>
              }
            />
            <Button type="submit" disabled={isPending}>
              {isPending ? "Создание..." : "Создать"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
