# SocialApp Frontend - Setup Guide

## 🎯 Quick Start

### 1. Prerequisites
- Node.js 18+ installed
- Your ASP.NET backend running on `http://localhost:5000`

### 2. Installation
```bash
cd social-app-frontend
npm install
```

### 3. Environment Configuration
The `.env.local` file is already created with default values:
```env
NEXT_PUBLIC_API_URL=http://localhost:5000/api
```

If your backend runs on a different port, update this value.

### 4. Start Development Server
```bash
npm run dev
```

Visit [http://localhost:3000](http://localhost:3000)

## 📋 Available Pages

### Public Pages (No Auth Required)
- **Landing Page**: `http://localhost:3000/`
- **Login**: `http://localhost:3000/login`
- **Register**: `http://localhost:3000/register`

### Protected Pages (Auth Required - Currently Mock)
- **Feed**: `http://localhost:3000/feed`
- **Profile**: `http://localhost:3000/profile/johndoe`

## 🔗 Backend Integration Steps

### Step 1: Update API Base URL
If your backend is not running on `http://localhost:5000`, update `.env.local`:
```env
NEXT_PUBLIC_API_URL=http://your-backend-url/api
```

### Step 2: Test Backend Connection
The API client is configured in `src/lib/api.ts`. It includes:
- Automatic JWT token handling
- Token refresh logic
- CORS support

### Step 3: Update API Endpoints
If your backend routes differ from the defaults, update `src/lib/queries.ts`:

```typescript
// Example: If your login endpoint is /Auth/Login instead of /auth/login
export const authApi = {
  login: async (credentials: LoginCredentials): Promise<AuthResponse> => {
    const { data } = await api.post('/Auth/Login', credentials); // Updated
    return data;
  },
  // ... other methods
};
```

### Step 4: Connect Authentication Pages
Update the login/register pages to use real API calls:

**In `src/app/(auth)/login/page.tsx`**:
```typescript
import { authApi } from '@/lib/queries';

const handleSubmit = async (e: React.FormEvent) => {
  e.preventDefault();
  setIsLoading(true);
  
  try {
    const response = await authApi.login({ email, password });
    localStorage.setItem('accessToken', response.accessToken);
    localStorage.setItem('refreshToken', response.refreshToken);
    // Redirect to feed
    window.location.href = '/feed';
  } catch (error) {
    console.error('Login failed:', error);
    // Show error message
  } finally {
    setIsLoading(false);
  }
};
```

### Step 5: Implement Protected Routes
Add authentication check in `src/app/(main)/layout.tsx`:

```typescript
"use client";

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Header } from "@/components/layout/header";

export default function MainLayout({ children }: { children: React.ReactNode }) {
  const router = useRouter();

  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    if (!token) {
      router.push('/login');
    }
  }, [router]);

  return (
    <div className="min-h-screen">
      <Header />
      <main>{children}</main>
    </div>
  );
}
```

## 🎨 Customization

### Change Colors
Edit `src/app/globals.css`:
```css
:root {
  --brand: #0072BB;        /* Primary color */
  --brand-light: #2A87FF;  /* Lighter variant */
  --brand-dark: #005CD5;   /* Darker variant */
}
```

### Change Fonts
Update the Google Fonts import in `src/app/globals.css`:
```css
@import url('https://fonts.googleapis.com/css2?family=YourFont:wght@400;700&display=swap');
```

Then update the CSS variables:
```css
:root {
  --font-urbanist: 'YourFont', sans-serif;
}
```

## 🐛 Troubleshooting

### CORS Issues
If you get CORS errors, ensure your backend has CORS configured:
```csharp
// In your ASP.NET Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCors", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
```

### API Connection Failed
1. Check if backend is running: `http://localhost:5000/api`
2. Verify `NEXT_PUBLIC_API_URL` in `.env.local`
3. Check browser console for error messages
4. Ensure backend CORS allows `http://localhost:3000`

### Images Not Loading
If using Next.js Image component with external URLs, add domains to `next.config.ts`:
```typescript
images: {
  remotePatterns: [
    {
      protocol: 'https',
      hostname: 'your-image-domain.com',
    },
  ],
},
```

## 📦 Production Build

### Build for Production
```bash
npm run build
```

### Start Production Server
```bash
npm start
```

### Environment Variables for Production
Create `.env.production`:
```env
NEXT_PUBLIC_API_URL=https://your-production-api.com/api
NEXTAUTH_URL=https://your-production-domain.com
NEXTAUTH_SECRET=your-production-secret-key
```

## 🚀 Deployment

### Vercel (Recommended)
1. Push code to GitHub
2. Import project in Vercel
3. Add environment variables
4. Deploy

### Other Platforms
- **Netlify**: Use `npm run build` and deploy `out/` folder
- **AWS/Azure**: Use Docker or static hosting
- **Self-hosted**: Use PM2 or similar process manager

## 📚 Next Features to Implement

1. **Real-time Updates**: WebSocket integration
2. **Image Upload**: File upload for posts
3. **Notifications**: Real-time notification system
4. **Search**: User and post search
5. **Direct Messages**: Chat functionality
6. **Stories**: Instagram-style stories
7. **Dark Mode**: Complete dark theme
8. **Mobile App**: React Native version

## 🤝 Need Help?

Check the main README.md for more details or refer to:
- Next.js Documentation: https://nextjs.org/docs
- Tailwind CSS: https://tailwindcss.com/docs
- shadcn/ui: https://ui.shadcn.com

---

Happy coding! 🎉