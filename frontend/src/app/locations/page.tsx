import { LocationsList } from "@/widgets/locations-list";

export default async function LocationsRoute({
  searchParams,
}: PageProps<"/locations">) {
  const { page: pageParam } = await searchParams;
  const page = Number(Array.isArray(pageParam) ? pageParam[0] : pageParam) || 1;

  return <LocationsList page={page} />;
}
