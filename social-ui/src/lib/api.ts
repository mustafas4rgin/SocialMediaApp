export const API_BASE =
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5084";

export type UserDto = {
  id: number;
  firstName: string;
  lastName: string;
};

export type UsersResponse = {
  message: string;
  user: UserDto[];
};

export async function getUsers(): Promise<UserDto[]> {

  const res = await fetch(`${API_BASE}/users`, {
    // auth varsa buraya token ekleriz
    cache: "no-store",
  });

  if (!res.ok) {
    throw new Error(`Users fetch failed: ${res.status}`);
  }

  const data = (await res.json()) as UsersResponse;
  return data.user ?? [];
}
