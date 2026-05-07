# MAUI Shopping App (Supabase)

.NET MAUI shopping app using MVVM and Supabase (PostgREST) for profile and shopping cart storage.

## Features

- Shopping List landing page with seeded fictitious shopping items
- Add item to shopping cart with stock-limit protection
- Shopping Cart page with remove-item support
- Profile page that preloads saved data and saves updates
- Cart items linked to the profile (`profile_id`)
- Fallback in-memory data mode when Supabase environment variables are not configured

## Database design (Supabase)

Run this SQL in Supabase SQL editor:

```sql
create table if not exists profiles (
  id integer generated always as identity primary key,
  name text,
  surname text,
  email_address text,
  bio text
);

create table if not exists shopping_items (
  id integer generated always as identity primary key,
  name text not null,
  description text not null,
  price numeric(10,2) not null check (price >= 0),
  stock_quantity integer not null check (stock_quantity >= 0)
);

create table if not exists shopping_cart_items (
  id integer generated always as identity primary key,
  profile_id integer not null references profiles(id) on delete cascade,
  shopping_item_id integer not null references shopping_items(id) on delete cascade,
  quantity integer not null check (quantity > 0),
  unique(profile_id, shopping_item_id)
);
```

## Supabase configuration

Set environment variables before running:

- `SUPABASE_URL`
- `SUPABASE_ANON_KEY`

If these are not set, the app runs with local fallback data.

## Project structure

- `Ass5/Services/SupabaseShoppingDataService.cs` - Supabase REST + local fallback data service
- `Ass5/ViewModels/*` - MVVM view models for Profile, Shopping List, and Cart
- `Ass5/ShoppingListPage.xaml` - landing page shopping UI
- `Ass5/ShoppingCartPage.xaml` - cart UI
- `Ass5/ProfilePage.xaml` - profile UI
- `Ass5/AppShell.xaml` - tab navigation between Shop, Cart, Profile

## Build

This project targets MAUI platforms and requires MAUI workloads (for example `maui-android`) installed in your environment.
