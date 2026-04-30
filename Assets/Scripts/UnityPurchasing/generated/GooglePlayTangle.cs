// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("SMeM5yfRtTVAOpjJP0ZC0/mtWOXFi9924TLsDkEucqHX7bCvyda57eclIjPfBnZNbRMXE3CXQhsWeG7Kc8FCYXNORUppxQvFtE5CQkJGQ0AYAaN010+VQT/qxeCR7GLF0kSAC5pycqqVTrfNmspoq7CZ9gWrYkd2SK0Yddr3h2roTAMvfyZJFWrRZVgD2R9OIiYR6QvMwBVM6ObFc8bfL9Q/Mcfa/ifLH/Tq9g8JpXf74F5aZ6Fg3kKlU/9cksqxtG9NdODNM/AlcvgMPl6h6p/4e2zsHrsnraD/vdXb9p2r+EmpLle3eG4i19DBbJtdwUJMQ3PBQklBwUJCQ4FQ5pErn3Pb8j+w5W9481nSGm7LAMUT6OQa/9DX2qCn8qh05EFAQkNC");
        private static int[] order = new int[] { 8,9,6,8,4,12,12,12,13,13,10,12,13,13,14 };
        private static int key = 67;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
