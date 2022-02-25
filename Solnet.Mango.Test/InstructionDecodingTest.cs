using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Mango.Models;
using Solnet.Programs;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Mango.Test
{
    [TestClass]
    public class InstructionDecodingTest
    {
        private const string CreateInitAccountAndAddInfoMessage = "AgAEBgpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4xLDRRtH4Ze1GxzHBPIYFJR6C3wI1sYogL3OugLMKQb0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMohIC1MXuUx1BqubENkb/BfDUR2wytGaIy8quZuGSIhBqfVFxksXFEhjMlMPUrxf1ja7gibof1E49vZigAAAAA5kw4m3RTlrY1U1TDoqPHph4SQpWHgwU5NDOx8pbUuMU/FoRaGfwosKTudIQhkxNpfx+mTRQxczI0V3JV+ojoSAwICAAE0AAAAAIDV1QEAAAAAyBAAAAAAAAA5kw4m3RTlrY1U1TDoqPHph4SQpWHgwU5NDOx8pbUuMQUEAwEABAQBAAAABQMDAQAkIgAAAA4AAAAAAAAAU29sbmV0IFRlc3QgdjEAAAAAAAAAAAAA";

        private const string CreateAccountAndAddInfoMessage = "AQACBQpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4yiEgLUxe5THUGq5sQ2Rv8F8NRHbDK0ZojLyq5m4ZIiGYAxig2FEvystJRkjr1kfkvxGi/EKTL1QaZFDVZrfaJAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOZMOJt0U5a2NVNUw6Kjx6YeEkKVh4MFOTQzsfKW1LjHS6qj2L5fLL9vU1O/8M8N6i3y85WY4/YMyCEgUJ/1Z4AIEBAECAAMMNwAAAAEAAAAAAAAABAMBAgAkIgAAAA4AAAAAAAAAU29sbmV0IFRlc3QgdjEAAAAAAAAAAAAA";

        private const string CloseMangoAccountMessage = "AQABBApz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4yiEgLUxe5THUGq5sQ2Rv8F8NRHbDK0ZojLyq5m4ZIiGYAxig2FEvystJRkjr1kfkvxGi/EKTL1QaZFDVZrfaJDmTDibdFOWtjVTVMOio8emHhJClYeDBTk0M7HyltS4xWdY2tSdsVr37+ParY6/0tJm8XhnXb9ms0Ov6K7RHWcEBAwMBAgAEMgAAAA==";

        private const string InitAdvancedOrdersMessage = "AQADBgpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4mAMYoNhRL8rLSUZI69ZH5L8RovxCky9UGmRQ1Wa32iSOIIphRK3htxlu/gwl4YwvBZ+2yxfdpAASUWQaPM0R78ohIC1MXuUx1BqubENkb/BfDUR2wytGaIy8quZuGSIhAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA5kw4m3RTlrY1U1TDoqPHph4SQpWHgwU5NDOx8pbUuMXy12+8ECy3WxLFp1gBUEV1KuwAU3wxNmQem1MT+sFitAQUFAwEAAgQEKgAAAA==";

        private const string WrapSolAndDepositMessage = "AgAIDQpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d43rGr6T5usSg6wVbs80KnsOrqyvkcFOA8AtxozPp9YOeYAxig2FEvystJRkjr1kfkvxGi/EKTL1QaZFDVZrfaJGSPqyqI0+LcfVurs+aOtyil7HlR7UCvOrrkjh5oSviFwrtWmZakLyaBG9mutu/THU9fRnS3TWww1wfF3dybQEwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAabiFf+q4GE+2h/Y0YYwDXaxDncGus7VZig8AAAAAABBqfVFxksXFEhjMlMPUrxf1ja7gibof1E49vZigAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqcohIC1MXuUx1BqubENkb/BfDUR2wytGaIy8quZuGSIhc1enabCRStasrnNU4+aP4LPFLTp9LFYZTQ1yFH/LwMVr5hhZPwoXhqySnHIzG5+InJNPrMxz0DGtN+8HGkoZqDmTDibdFOWtjVTVMOio8emHhJClYeDBTk0M7HyltS4xBW6bXOW5Be0fh+euGYrZDLq+jp3ZKBnCVJMzTh5oOA0EBQIAATQAAAAAMGAuAAAAAAClAAAAAAAAAAbd9uHXZaGT2cvhRs7reawctIXtX1s3kTqM9YV+/wCpCAQBBgAHAQEMCQkCAAoLAwQIAQwCAAAAQEIPAAAAAAAIAwEAAAEJ";

        private const string WrapSolAndWithdrawMessage = "AgAIDgpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4Oo7h2TyF/61UJKJrAdyalq3eNaCKZ0cXWJAo5rtLRa7KISAtTF7lMdQarmxDZG/wXw1EdsMrRmiMvKrmbhkiIZgDGKDYUS/Ky0lGSOvWR+S/EaL8QpMvVBpkUNVmt9okZI+rKojT4tx9W6uz5o63KKXseVHtQK86uuSOHmhK+IXCu1aZlqQvJoEb2a6279MdT19GdLdNbDDXB8Xd3JtATAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABpuIV/6rgYT7aH9jRhjANdrEOdwa6ztVmKDwAAAAAAEGp9UXGSxcUSGMyUw9SvF/WNruCJuh/UTj29mKAAAAAAbd9uHXZaGT2cvhRs7reawctIXtX1s3kTqM9YV+/wCpc1enabCRStasrnNU4+aP4LPFLTp9LFYZTQ1yFH/LwMVr5hhZPwoXhqySnHIzG5+InJNPrMxz0DGtN+8HGkoZqKcwEDDqWqCTX/YYsLoZIm4/AOjtdu54FMVywS+aYKOBOZMOJt0U5a2NVNUw6Kjx6YeEkKVh4MFOTQzsfKW1LjGwFmCE8fs5oQ/HhVa/6Ya9ebwSLi/k71q/DokopdMNMwQGAgABNAAAAADwHR8AAAAAAKUAAAAAAAAABt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/AKkJBAEHAAgBAQ0ZAgMACgsEBQEMCQYGBgYGBgYGBgYGBgYGBg0DAAAAAMqaOwAAAAAACQMBAAABCQ==";

        private const string WrapSolAndWithdrawAllowBorrowMessage = "AgAIDgpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4gwkMpjQVXebQuVIFAsRpPbbEzp0Q1wmCtVyIC8d/G+XKISAtTF7lMdQarmxDZG/wXw1EdsMrRmiMvKrmbhkiIZgDGKDYUS/Ky0lGSOvWR+S/EaL8QpMvVBpkUNVmt9okZI+rKojT4tx9W6uz5o63KKXseVHtQK86uuSOHmhK+IXCu1aZlqQvJoEb2a6279MdT19GdLdNbDDXB8Xd3JtATAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABpuIV/6rgYT7aH9jRhjANdrEOdwa6ztVmKDwAAAAAAEGp9UXGSxcUSGMyUw9SvF/WNruCJuh/UTj29mKAAAAAAbd9uHXZaGT2cvhRs7reawctIXtX1s3kTqM9YV+/wCpc1enabCRStasrnNU4+aP4LPFLTp9LFYZTQ1yFH/LwMVr5hhZPwoXhqySnHIzG5+InJNPrMxz0DGtN+8HGkoZqKcwEDDqWqCTX/YYsLoZIm4/AOjtdu54FMVywS+aYKOBOZMOJt0U5a2NVNUw6Kjx6YeEkKVh4MFOTQzsfKW1LjG4A0f1cV2J8dGKdVS0yR2NK0CgOyDfROkvCASQjunbrAQGAgABNAAAAADwHR8AAAAAAKUAAAAAAAAABt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/AKkJBAEHAAgBAQ0ZAgMACgsEBQEMCQYGBgYGBgYGBgYGBgYGBg0DAAAAAMqaOwAAAAABCQMBAAABCQ==";

        private const string CreateSpotOpenOrdersMessage = "AgAHCgpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4GfbgTQFSq1bBywHZg3z8cJ1HO4R6pxEHO/H/gFV8doGYAxig2FEvystJRkjr1kfkvxGi/EKTL1QaZFDVZrfaJAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAyiEgLUxe5THUGq5sQ2Rv8F8NRHbDK0ZojLyq5m4ZIiG1vZ7LZ502Gt8wb/WL0dyrOS4eySbPnOiFlVUBjsVtH0mnaoE/owmoOSrNJAggDkGSNpQEzcFKVtGs7saLSwrOpzAQMOpaoJNf9hiwuhkibj8A6O127ngUxXLBL5pgo4EGp9UXGSxcUSGMyUw9SvF/WNruCJuh/UTj29mKAAAAADmTDibdFOWtjVTVMOio8emHhJClYeDBTk0M7HyltS4xu9tug4cYKq1RIYrZZ8TxVZrP3GmHB31w0Nl6iprkj5wCAwIAATQAAAAAQGlkAQAAAACcDAAAAAAAALW9nstnnTYa3zBv9YvR3Ks5Lh7JJs+c6IWVVQGOxW0fCQgEAgAFAQYHCAQgAAAA";

        private const string CloseSpotOpenOrdersMessage = "AQAFCApz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4mAMYoNhRL8rLSUZI69ZH5L8RovxCky9UGmRQ1Wa32iQZ9uBNAVKrVsHLAdmDfPxwnUc7hHqnEQc78f+AVXx2gcohIC1MXuUx1BqubENkb/BfDUR2wytGaIy8quZuGSIhtb2ey2edNhrfMG/1i9HcqzkuHskmz5zohZVVAY7FbR9Jp2qBP6MJqDkqzSQIIA5BkjaUBM3BSlbRrO7Gi0sKzqcwEDDqWqCTX/YYsLoZIm4/AOjtdu54FMVywS+aYKOBOZMOJt0U5a2NVNUw6Kjx6YeEkKVh4MFOTQzsfKW1LjHZnEjWwUtqc3OoG9M6/Jf1GCR1ZZYD3uMOtNJWDBQpXwEHBwMBAAQCBQYEMwAAAA==";

        private const string PlaceSpotOrderSettleFundsMessage = "AQAMGgpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4mAMYoNhRL8rLSUZI69ZH5L8RovxCky9UGmRQ1Wa32iRJp2qBP6MJqDkqzSQIIA5BkjaUBM3BSlbRrO7Gi0sKznG9rhza62LsfeMx4oQJfnrM1zXLi1yzboXN3th2KFoZchN6/Zmex7S4q4ayJ1B72yw6cmbByhRKGehHPNuyS8A3BDaF5kIMsfFl5dNxufsCAkd94Wwr+BNr8I4vpx9IJy6FSyQs7fg5PmBvSSoK8HyGzz3xWdLjBpKMSZA8069AyyIbgQM/sQqGaQqQRORr15V01hay51FkEtUvmGc8uifxzCBCuGuj4IRudXFAxA24VY/Jzb9N6/iCDZuWNEBuImSPqyqI0+LcfVurs+aOtyil7HlR7UCvOrrkjh5oSviFwrtWmZakLyaBG9mutu/THU9fRnS3TWww1wfF3dybQEz87p1yiW7UvGC2IGNn9dcxsRw7kIZpzeJvJxaxG1i+5ozpEcBPi0u5jqirLdLfV1SiI0Cyp6K0y6s6FrdekCTncDmn0Nd3Y84NZcsnHKXuZpXddV6Dj+Jzl27WlW28CufKISAtTF7lMdQarmxDZG/wXw1EdsMrRmiMvKrmbhkiIXNXp2mwkUrWrK5zVOPmj+CzxS06fSxWGU0NchR/y8DFtb2ey2edNhrfMG/1i9HcqzkuHskmz5zohZVVAY7FbR9r5hhZPwoXhqySnHIzG5+InJNPrMxz0DGtN+8HGkoZqPSxkNhSWm0gGD1FbRnOy4dfA4wuFM8iFNR7OovlF8k7Bt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/AKmnMBAw6lqgk1/2GLC6GSJuPwDo7XbueBTFcsEvmmCjgQan1RcZLFxRIYzJTD1K8X9Y2u4Im6H9ROPb2YoAAAAADv/CwaXqWv3R1/YH1JSz3kR8dn7y9SoqzyMDr+/CiOxkyapPjyS8o+lRmPtjyX5TrIrvEE+8BxUDE7INgNaSyAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOZMOJt0U5a2NVNUw6Kjx6YeEkKVh4MFOTQzsfKW1LjEE9559n15k5f6tWLTBkpi7A0N63Fo7Gby6+cguIm+wlAIZJg4BAA8QAgMEBQYHCBEJChILDBMUFRYXGBgYDRgYGBgYGBgYGBgYMgkAAAABAAAAKJoBAAAAAAD6AAAAAAAAAEBSdpwAAAAAAgAAAAEAAABAQg8AAAAAAP//GRIODwABEAINFAcIEQkSCwoMFhMEEwAAAA==";

        private const string PlaceSpotOrder2SettleFundsMessage = "AQALGQpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4mAMYoNhRL8rLSUZI69ZH5L8RovxCky9UGmRQ1Wa32iRJp2qBP6MJqDkqzSQIIA5BkjaUBM3BSlbRrO7Gi0sKznG9rhza62LsfeMx4oQJfnrM1zXLi1yzboXN3th2KFoZchN6/Zmex7S4q4ayJ1B72yw6cmbByhRKGehHPNuyS8A3BDaF5kIMsfFl5dNxufsCAkd94Wwr+BNr8I4vpx9IJy6FSyQs7fg5PmBvSSoK8HyGzz3xWdLjBpKMSZA8069AyyIbgQM/sQqGaQqQRORr15V01hay51FkEtUvmGc8uifxzCBCuGuj4IRudXFAxA24VY/Jzb9N6/iCDZuWNEBuImSPqyqI0+LcfVurs+aOtyil7HlR7UCvOrrkjh5oSviFwrtWmZakLyaBG9mutu/THU9fRnS3TWww1wfF3dybQEz87p1yiW7UvGC2IGNn9dcxsRw7kIZpzeJvJxaxG1i+5ozpEcBPi0u5jqirLdLfV1SiI0Cyp6K0y6s6FrdekCTncDmn0Nd3Y84NZcsnHKXuZpXddV6Dj+Jzl27WlW28CufKISAtTF7lMdQarmxDZG/wXw1EdsMrRmiMvKrmbhkiIXNXp2mwkUrWrK5zVOPmj+CzxS06fSxWGU0NchR/y8DFtb2ey2edNhrfMG/1i9HcqzkuHskmz5zohZVVAY7FbR9r5hhZPwoXhqySnHIzG5+InJNPrMxz0DGtN+8HGkoZqPSxkNhSWm0gGD1FbRnOy4dfA4wuFM8iFNR7OovlF8k7Bt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/AKmnMBAw6lqgk1/2GLC6GSJuPwDo7XbueBTFcsEvmmCjgQ7/wsGl6lr90df2B9SUs95EfHZ+8vUqKs8jA6/vwojsZMmqT48kvKPpUZj7Y8l+U6yK7xBPvAcVAxOyDYDWksgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADmTDibdFOWtjVTVMOio8emHhJClYeDBTk0M7HyltS4xNcJbtIHMncYfmRjROsmyY2tqLGp+pgGiqLe+dBBWtjoCGCUOAQAPEAIDBAUGBwgRCQoSCwwTFBUWFxcXDRcXFxcXFxcXFxcXMikAAAABAAAAKJoBAAAAAAD6AAAAAAAAAEBSdpwAAAAAAgAAAAEAAABAQg8AAAAAAP//GBIODwABEAINFAcIEQkSCwoMFRMEEwAAAA==";

        private const string CancelSpotOrderMessage = "AQAFCwpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4SadqgT+jCag5Ks0kCCAOQZI2lATNwUpW0azuxotLCs5xva4c2uti7H3jMeKECX56zNc1y4tcs26Fzd7YdihaGXITev2Znse0uKuGsidQe9ssOnJmwcoUShnoRzzbskvAcDmn0Nd3Y84NZcsnHKXuZpXddV6Dj+Jzl27WlW28CucuhUskLO34OT5gb0kqCvB8hs898VnS4waSjEmQPNOvQMohIC1MXuUx1BqubENkb/BfDUR2wytGaIy8quZuGSIhmAMYoNhRL8rLSUZI69ZH5L8RovxCky9UGmRQ1Wa32iS1vZ7LZ502Gt8wb/WL0dyrOS4eySbPnOiFlVUBjsVtH6cwEDDqWqCTX/YYsLoZIm4/AOjtdu54FMVywS+aYKOBOZMOJt0U5a2NVNUw6Kjx6YeEkKVh4MFOTQzsfKW1LjEkya3N1RsJL2KN9k1MUNX7N7lvklVQC/ixMHCRiCRqHwEKCgYABwgBAgMECQUYFAAAAAEAAAAF1UoAAAAAAJDQAwAAAAAA";

        private const string PlacePerpOrderMessage = "AQAFCwpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4mAMYoNhRL8rLSUZI69ZH5L8RovxCky9UGmRQ1Wa32iQ9dmVTKSYZag+O01pwnLcV1a7r3u6a0cY8x7rwMTjHZV1bU5cbj2puO4JDHMAiUZ9jWoNO+IbWCyIipn8n0f5zOHSa70IivD53UX4zgb7JC7c7a12AKmjrw3J2+hatSEerqA8Cp4Gf1w/c3bseRn5Vhg+dYleVYrleQsgDaFd3qsohIC1MXuUx1BqubENkb/BfDUR2wytGaIy8quZuGSIhc1enabCRStasrnNU4+aP4LPFLTp9LFYZTQ1yFH/LwMUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA5p9DXd2PODWXLJxyl7maV3XVeg4/ic5du1pVtvArnOZMOJt0U5a2NVNUw6Kjx6YeEkKVh4MFOTQzsfKW1LjG8QKfc5JmwYzbydrXvd9eV7JP4imaF3HKCylnqvhOKWAEKFwYBAAcCAwQFCAgICQgICAgICAgICAgIHwwAAADAKwAAAAAAAKhhAAAAAAAAQEIPAAAAAAAAAQA=";

        private const string PlacePerpOrderReduceOnlyMessage = "AQAFCwpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4mAMYoNhRL8rLSUZI69ZH5L8RovxCky9UGmRQ1Wa32iQ9dmVTKSYZag+O01pwnLcV1a7r3u6a0cY8x7rwMTjHZV1bU5cbj2puO4JDHMAiUZ9jWoNO+IbWCyIipn8n0f5zOHSa70IivD53UX4zgb7JC7c7a12AKmjrw3J2+hatSEerqA8Cp4Gf1w/c3bseRn5Vhg+dYleVYrleQsgDaFd3qsohIC1MXuUx1BqubENkb/BfDUR2wytGaIy8quZuGSIhc1enabCRStasrnNU4+aP4LPFLTp9LFYZTQ1yFH/LwMUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA5p9DXd2PODWXLJxyl7maV3XVeg4/ic5du1pVtvArnOZMOJt0U5a2NVNUw6Kjx6YeEkKVh4MFOTQzsfKW1LjGvcSifGstfGYT/GEJ+a+BRbAJ62LtYrAsiusZaBJLnkgEKFwYBAAcCAwQFCAgICQgICAgICAgICAgIHwwAAAAEKQAAAAAAAKhhAAAAAAAAQEIPAAAAAAABAQE=";

        private const string SettleFeesMessage = "AQAGDApz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4PXZlUykmGWoPjtNacJy3FdWu697umtHGPMe68DE4x2WYAxig2FEvystJRkjr1kfkvxGi/EKTL1QaZFDVZrfaJPzunXKJbtS8YLYgY2f11zGxHDuQhmnN4m8nFrEbWL7mjOkRwE+LS7mOqKst0t9XVKIjQLKnorTLqzoWt16QJOc8TQ4J2EPl9BDMF+zkq/DCJmIcxEjojfFW/ZpOiHHc6sohIC1MXuUx1BqubENkb/BfDUR2wytGaIy8quZuGSIhc1enabCRStasrnNU4+aP4LPFLTp9LFYZTQ1yFH/LwMX0sZDYUlptIBg9RW0ZzsuHXwOMLhTPIhTUezqL5RfJO6cwEDDqWqCTX/YYsLoZIm4/AOjtdu54FMVywS+aYKOBBt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/AKk5kw4m3RTlrY1U1TDoqPHph4SQpWHgwU5NDOx8pbUuMXbH15BXMTE9b0EDMhCHjJirqhVm0ZQt0Uw8u3N+OQVWAQsKBgcBAggDBAUJCgQdAAAA";

        private const string SetDelegateMessage = "AQADBQpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4mAMYoNhRL8rLSUZI69ZH5L8RovxCky9UGmRQ1Wa32iTKISAtTF7lMdQarmxDZG/wXw1EdsMrRmiMvKrmbhkiIUdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyOZMOJt0U5a2NVNUw6Kjx6YeEkKVh4MFOTQzsfKW1LjFCOnISNMoN1q1oaY51oyod00EnC8RaGBYrGcg8VhA8LwEEBAIBAAMEOgAAAA==";

        private const string AddPerpTriggerOrderMessage = "AQAGCApz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4rQqXXruIOVtp8L9IgE6vrVLzYNobX3u6/Rbj/+6kqIjKISAtTF7lMdQarmxDZG/wXw1EdsMrRmiMvKrmbhkiIVTo0h0JftHn9tx6NSAOOqytyt4VksgHKcuqfxgcBhksc1enabCRStasrnNU4+aP4LPFLTp9LFYZTQ1yFH/LwMU9dmVTKSYZag+O01pwnLcV1a7r3u6a0cY8x7rwMTjHZQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOZMOJt0U5a2NVNUw6Kjx6YeEkKVh4MFOTQzsfKW1LjFLew9i3W5WAGjIb8A2ki3kJqo/xqGKFgUQ3QYnHjiZoQEHFgIDAAEEBQYGBgYGBgYGBgYGBgYGBgYwKwAAAAEAAQFAQg8AAAAAAKCGAQAAAAAAoIYBAAAAAAAAAAAAAACghgEAAAAAAAAA";

        private const string RemoveAdvancedOrderMessage = "AQAEBgpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4rQqXXruIOVtp8L9IgE6vrVLzYNobX3u6/Rbj/+6kqIjKISAtTF7lMdQarmxDZG/wXw1EdsMrRmiMvKrmbhkiIVTo0h0JftHn9tx6NSAOOqytyt4VksgHKcuqfxgcBhksAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA5kw4m3RTlrY1U1TDoqPHph4SQpWHgwU5NDOx8pbUuMVy7776PgCLScUwD+RHspt3GxeMeP3YLDofTNwAmOLb5AgUFAgMAAQQFLAAAAAAFBQIDAAEEBSwAAAAB";

        private const string RegisterReferrerIdMessage = "AQADBgpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4VOjSHQl+0ef23Ho1IA46rK3K3hWSyAcpy6p/GBwGGSyOp6NBx6ory8mCYmEEOJfjeV5Cnluo6hqEWgoNXV0QIMohIC1MXuUx1BqubENkb/BfDUR2wytGaIy8quZuGSIhAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA5kw4m3RTlrY1U1TDoqPHph4SQpWHgwU5NDOx8pbUuMfzcPp0NtGiciNbubOEdIMRdL8iIdgd6VlVb297cJPymAQUFAwECAAQkPwAAAAsAAAAAAAAATWFuZ28gU2hhcnAAAAAAAAAAAAAAAAAA";

        private const string CreateAddInfoAndSetReferrerMessage = "AQADBwpz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4yiEgLUxe5THUGq5sQ2Rv8F8NRHbDK0ZojLyq5m4ZIiFUuwWRySJQNgY58mxU+b8uVIuhNTll53Dwl92OZZe3JkTFd9tOlil5/rnLZPD7yWlc3nbyadEvaHdN1aiMKRWiAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA5kw4m3RTlrY1U1TDoqPHph4SQpWHgwU5NDOx8pbUuMZgDGKDYUS/Ky0lGSOvWR+S/EaL8QpMvVBpkUNVmt9okws4NuTwmaDTUgOHsTuXuQqpZ4EMZU8FTWTackpIQdPoDBQQBAgAEDDcAAAAEAAAAAAAAAAUDAQIAJCIAAABNYW5nbyBTaGFycCBSZWYgVGVzdAAAAAAAAAAAAAAAAAUHAQIAAwYABAQ+AAAA";

        private const string PlacePerpOrder2WithTifMessage = "AQAGDApz5x/t0hNl7QruhPzk4rIGR/001ey9oRXwI9JjP4d4mAMYoNhRL8rLSUZI69ZH5L8RovxCky9UGmRQ1Wa32iQ9dmVTKSYZag+O01pwnLcV1a7r3u6a0cY8x7rwMTjHZV1bU5cbj2puO4JDHMAiUZ9jWoNO+IbWCyIipn8n0f5zOHSa70IivD53UX4zgb7JC7c7a12AKmjrw3J2+hatSEerqA8Cp4Gf1w/c3bseRn5Vhg+dYleVYrleQsgDaFd3qsohIC1MXuUx1BqubENkb/BfDUR2wytGaIy8quZuGSIhc1enabCRStasrnNU4+aP4LPFLTp9LFYZTQ1yFH/LwMUKu59GUp19o2AwuBTxLZi4MI+qSdpEVVFQ9lCtgxJxQQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAcDmn0Nd3Y84NZcsnHKXuZpXddV6Dj+Jzl27WlW28Cuc5kw4m3RTlrY1U1TDoqPHph4SQpWHgwU5NDOx8pbUuMe8bteDdqrDGrEyKkxrmMa1J/gS5LCW1XuA0DUAEpGsxAQsYBgEABwIDBAUBCAkJCgkJCQkJCQkJCQkJMEAAAABMHQAAAAAAABAnAAAAAAAAwGh4BAAAAAABAAAAAAAAAG8UGGIAAAAAAAAA/w==";

        [ClassInitialize]
        public static void Setup(TestContext tc)
        {
            InstructionDecoder.Register(MangoProgram.DevNetProgramIdKeyV3, MangoProgram.Decode);
            // do not assert the program key of the decoded instruction because it defaults to mainnet
        }

        [TestMethod]
        public void DecodeCreateAndAddAccountInfo()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(CreateInitAccountAndAddInfoMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(3, ix.Count);
            Assert.AreEqual("Initialize Mango Account", ix[1].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[1].ProgramName);
            Assert.AreEqual(0, ix[1].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[1].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("EEoHecXUMTx5omC1cTHDuv7VKGTmzq8M5d86GnBHYMPS", ix[1].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[1].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual(SysVars.RentKey, ix[1].Values.GetValueOrDefault("Sysvar Rent").ToString());

            Assert.AreEqual("Add Mango Account Info", ix[2].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[2].ProgramName);
            Assert.AreEqual(0, ix[2].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[2].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("EEoHecXUMTx5omC1cTHDuv7VKGTmzq8M5d86GnBHYMPS", ix[2].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[2].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("\0\0\0\0\0\0\0Solnet Test v1", ix[2].Values.GetValueOrDefault("Account Info"));
        }

        [TestMethod]
        public void CreateAccountAndAddInfo()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(CreateAccountAndAddInfoMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(2, ix.Count);
            Assert.AreEqual("Create Mango Account", ix[0].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[0].ProgramName);
            Assert.AreEqual(0, ix[0].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[0].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[0].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[0].Values.GetValueOrDefault("Owner").ToString());

            Assert.AreEqual("Add Mango Account Info", ix[1].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[1].ProgramName);
            Assert.AreEqual(0, ix[1].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[1].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[1].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[1].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("\0\0\0\0\0\0\0Solnet Test v1", ix[1].Values.GetValueOrDefault("Account Info"));
        }

        [TestMethod]
        public void CloseMangoAccount()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(CloseMangoAccountMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, ix.Count);
            Assert.AreEqual("Close Mango Account", ix[0].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[0].ProgramName);
            Assert.AreEqual(0, ix[0].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[0].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[0].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[0].Values.GetValueOrDefault("Owner").ToString());
        }

        [TestMethod]
        public void InitAdvancedOrders()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(InitAdvancedOrdersMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, ix.Count);
            Assert.AreEqual("Initialize Advanced Orders Account", ix[0].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[0].ProgramName);
            Assert.AreEqual(0, ix[0].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[0].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[0].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[0].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("AZofyy49f3sY6bt3F1vgMse92eZdFuCM8jkV1USofneA", ix[0].Values.GetValueOrDefault("Advanced Orders").ToString());
            Assert.AreEqual(SystemProgram.ProgramIdKey, ix[0].Values.GetValueOrDefault("System Program").ToString());
        }

        [TestMethod]
        public void WrapSolAndDeposit()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(WrapSolAndDepositMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(4, ix.Count);
            Assert.AreEqual("Deposit", ix[2].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[2].ProgramName);
            Assert.AreEqual(0, ix[2].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[2].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[2].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[2].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z", ix[2].Values.GetValueOrDefault("Mango Cache").ToString());
            Assert.AreEqual("8GC81raaLjhTx3yedctxCJW46qdmmSRybH2s1eFYFFxT", ix[2].Values.GetValueOrDefault("Root Bank").ToString());
            Assert.AreEqual("7mYqCavd1K24fnL3oKTpX3YM66W5gfikmVHJWM3nrWKe", ix[2].Values.GetValueOrDefault("Node Bank").ToString());
            Assert.AreEqual("E79n2SiixBFrQqrq8JDCe1ZJXHcVADaQofe246b9qaRy", ix[2].Values.GetValueOrDefault("Vault").ToString());
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", ix[2].Values.GetValueOrDefault("Token Program").ToString());
            Assert.AreEqual("FzJeGP1pscG95Ykjh6ftAwzd9HWvasGDtF2cJM9cPEJS", ix[2].Values.GetValueOrDefault("Owner Token Account").ToString());
            Assert.AreEqual("1000000", ix[2].Values.GetValueOrDefault("Quantity").ToString());
        }

        [TestMethod]
        public void WrapSolAndWithdraw()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(WrapSolAndWithdrawMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(4, ix.Count);
            Assert.AreEqual("Withdraw", ix[2].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[2].ProgramName);
            Assert.AreEqual(0, ix[2].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[2].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[2].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[2].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z", ix[2].Values.GetValueOrDefault("Mango Cache").ToString());
            Assert.AreEqual("8GC81raaLjhTx3yedctxCJW46qdmmSRybH2s1eFYFFxT", ix[2].Values.GetValueOrDefault("Root Bank").ToString());
            Assert.AreEqual("7mYqCavd1K24fnL3oKTpX3YM66W5gfikmVHJWM3nrWKe", ix[2].Values.GetValueOrDefault("Node Bank").ToString());
            Assert.AreEqual("E79n2SiixBFrQqrq8JDCe1ZJXHcVADaQofe246b9qaRy", ix[2].Values.GetValueOrDefault("Vault").ToString());
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", ix[2].Values.GetValueOrDefault("Token Program").ToString());
            Assert.AreEqual("4wb1i9N9Kz7QK4UGscWANi2rLhbe9HnCJPkUMvkA61Q1", ix[2].Values.GetValueOrDefault("Owner Token Account").ToString());
            Assert.AreEqual("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax", ix[2].Values.GetValueOrDefault("Mango Signer").ToString());
            Assert.AreEqual("1000000000", ix[2].Values.GetValueOrDefault("Quantity").ToString());
            Assert.AreEqual("False", ix[2].Values.GetValueOrDefault("Allow Borrow").ToString());
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                Assert.AreEqual(SystemProgram.ProgramIdKey, ix[2].Values.GetValueOrDefault($"Open Orders Account {i + 1}").ToString());
            }
        }

        [TestMethod]
        public void WrapSolAndWithdrawAllowBorrow()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(WrapSolAndWithdrawAllowBorrowMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(4, ix.Count);
            Assert.AreEqual("Withdraw", ix[2].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[2].ProgramName);
            Assert.AreEqual(0, ix[2].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[2].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[2].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[2].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z", ix[2].Values.GetValueOrDefault("Mango Cache").ToString());
            Assert.AreEqual("8GC81raaLjhTx3yedctxCJW46qdmmSRybH2s1eFYFFxT", ix[2].Values.GetValueOrDefault("Root Bank").ToString());
            Assert.AreEqual("7mYqCavd1K24fnL3oKTpX3YM66W5gfikmVHJWM3nrWKe", ix[2].Values.GetValueOrDefault("Node Bank").ToString());
            Assert.AreEqual("E79n2SiixBFrQqrq8JDCe1ZJXHcVADaQofe246b9qaRy", ix[2].Values.GetValueOrDefault("Vault").ToString());
            Assert.AreEqual(TokenProgram.ProgramIdKey, ix[2].Values.GetValueOrDefault("Token Program").ToString());
            Assert.AreEqual("9pWQt8hxBdhufevcXibqH7EWVWtWewNy7EA5mBENRRDW", ix[2].Values.GetValueOrDefault("Owner Token Account").ToString());
            Assert.AreEqual("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax", ix[2].Values.GetValueOrDefault("Mango Signer").ToString());
            Assert.AreEqual("1000000000", ix[2].Values.GetValueOrDefault("Quantity").ToString());
            Assert.AreEqual("True", ix[2].Values.GetValueOrDefault("Allow Borrow").ToString());
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                Assert.AreEqual(SystemProgram.ProgramIdKey, ix[2].Values.GetValueOrDefault($"Open Orders Account {i + 1}").ToString());
            }
        }

        [TestMethod]
        public void CreateSpotOpenOrders()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(CreateSpotOpenOrdersMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(2, ix.Count);
            Assert.AreEqual("Initialize Spot Open Orders Account", ix[1].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[1].ProgramName);
            Assert.AreEqual(0, ix[1].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[1].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[1].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[1].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("DESVgJVGajEgKGXhb6XmqDHGz3VjdgP7rEVESBgxmroY", ix[1].Values.GetValueOrDefault("Dex Program").ToString());
            Assert.AreEqual("2kMXoEVQabmHBxoRtkbzAUAMUvYtd5aGAKW9xdyeArwi", ix[1].Values.GetValueOrDefault("Open Orders Account").ToString());
            Assert.AreEqual("5xWpt56U1NCuHoAEtpLeUrQcxDkEpNfScjfLFaRzLPgR", ix[1].Values.GetValueOrDefault("Spot Market").ToString());
            Assert.AreEqual("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax", ix[1].Values.GetValueOrDefault("Mango Signer").ToString());
            Assert.AreEqual("SysvarRent111111111111111111111111111111111", ix[1].Values.GetValueOrDefault("Sysvar Rent").ToString());
        }

        [TestMethod]
        public void CloseSpotOpenOrders()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(CloseSpotOpenOrdersMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, ix.Count);
            Assert.AreEqual("Close Spot Open Orders Account", ix[0].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[0].ProgramName);
            Assert.AreEqual(0, ix[0].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[0].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[0].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[0].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("DESVgJVGajEgKGXhb6XmqDHGz3VjdgP7rEVESBgxmroY", ix[0].Values.GetValueOrDefault("Dex Program").ToString());
            Assert.AreEqual("2kMXoEVQabmHBxoRtkbzAUAMUvYtd5aGAKW9xdyeArwi", ix[0].Values.GetValueOrDefault("Open Orders Account").ToString());
            Assert.AreEqual("5xWpt56U1NCuHoAEtpLeUrQcxDkEpNfScjfLFaRzLPgR", ix[0].Values.GetValueOrDefault("Spot Market").ToString());
            Assert.AreEqual("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax", ix[0].Values.GetValueOrDefault("Mango Signer").ToString());
        }

        [TestMethod]
        public void PlaceSpotOrderSettleFunds()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(PlaceSpotOrderSettleFundsMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(2, ix.Count);
            Assert.AreEqual("Place Spot Order", ix[0].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[0].ProgramName);
            Assert.AreEqual(0, ix[0].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[0].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[0].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[0].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z", ix[0].Values.GetValueOrDefault("Mango Cache").ToString());
            Assert.AreEqual("DESVgJVGajEgKGXhb6XmqDHGz3VjdgP7rEVESBgxmroY", ix[0].Values.GetValueOrDefault("Dex Program").ToString());
            Assert.AreEqual("5xWpt56U1NCuHoAEtpLeUrQcxDkEpNfScjfLFaRzLPgR", ix[0].Values.GetValueOrDefault("Spot Market").ToString());
            Assert.AreEqual("8ezpneRznTJNZWFSLeQvtPCagpsUVWA7djLSzqp3Hx4p", ix[0].Values.GetValueOrDefault("Bids").ToString());
            Assert.AreEqual("8gJhxSwbLJkDQbqgzbJ6mDvJYnEVWB6NHWEN9oZZkwz7", ix[0].Values.GetValueOrDefault("Asks").ToString());
            Assert.AreEqual("4hm9fSbga3vosq4K4czPFwy1tmigc4io4ZLJqgC2s4Yv", ix[0].Values.GetValueOrDefault("Dex Request Queue").ToString());
            Assert.AreEqual("48be6VKEq86awgUjfvbKDmEzXr4WNR7hzDxfF6ZPptmd", ix[0].Values.GetValueOrDefault("Dex Event Queue").ToString());
            Assert.AreEqual("EfwvntttcP253P1g8Jqo18Rywyb33deGe69Txn5LSgwQ", ix[0].Values.GetValueOrDefault("Dex Base Vault").ToString());
            Assert.AreEqual("HGsnYZr6yodSHXHNKdtprooS9XP4h19hrzXfgJLuA3MB", ix[0].Values.GetValueOrDefault("Dex Quote Vault").ToString());
            Assert.AreEqual("8GC81raaLjhTx3yedctxCJW46qdmmSRybH2s1eFYFFxT", ix[0].Values.GetValueOrDefault("Base Root Bank").ToString());
            Assert.AreEqual("7mYqCavd1K24fnL3oKTpX3YM66W5gfikmVHJWM3nrWKe", ix[0].Values.GetValueOrDefault("Base Node Bank").ToString());
            Assert.AreEqual("E79n2SiixBFrQqrq8JDCe1ZJXHcVADaQofe246b9qaRy", ix[0].Values.GetValueOrDefault("Base Vault").ToString());
            Assert.AreEqual("HUBX4iwWEUK5VrXXXcB7uhuKrfT4fpu2T9iZbg712JrN", ix[0].Values.GetValueOrDefault("Quote Root Bank").ToString());
            Assert.AreEqual("J2Lmnc1e4frMnBEJARPoHtfpcohLfN67HdK1inXjTFSM", ix[0].Values.GetValueOrDefault("Quote Node Bank").ToString());
            Assert.AreEqual("AV4CuwdvnccZMXNhu9cSCx1mkpgHWcwWEJ7Yb8Xh8QMC", ix[0].Values.GetValueOrDefault("Quote Vault").ToString());
            Assert.AreEqual(TokenProgram.ProgramIdKey, ix[0].Values.GetValueOrDefault("Token Program").ToString());
            Assert.AreEqual("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax", ix[0].Values.GetValueOrDefault("Mango Signer").ToString());
            Assert.AreEqual(SysVars.RentKey, ix[0].Values.GetValueOrDefault("Sysvar Rent").ToString());
            Assert.AreEqual("21YuRgN6iHgsucfXT6Yzo2dV7dzuAdz2vxExahY3MueT", ix[0].Values.GetValueOrDefault("Dex Vault Signer").ToString());
            Assert.AreEqual("7nS8AgndAVYCfSTaXabwqcBWBc3xCLwwcPiMWzuTbfrf", ix[0].Values.GetValueOrDefault("Serum Vault").ToString());
            Assert.AreEqual("Sell", ix[0].Values.GetValueOrDefault("Side").ToString());
            Assert.AreEqual("105000", ix[0].Values.GetValueOrDefault("Price").ToString());
            Assert.AreEqual("250", ix[0].Values.GetValueOrDefault("Max Coin Quantity").ToString());
            Assert.AreEqual("2625000000", ix[0].Values.GetValueOrDefault("Max Price Coin Quantity").ToString());
            Assert.AreEqual("AbortTransaction", ix[0].Values.GetValueOrDefault("Self Trade Behavior").ToString());
            Assert.AreEqual("ImmediateOrCancel", ix[0].Values.GetValueOrDefault("Order Type").ToString());
            Assert.AreEqual("1000000", ix[0].Values.GetValueOrDefault("Client Order Id").ToString());
            Assert.AreEqual("65535", ix[0].Values.GetValueOrDefault("Limit").ToString());
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                if (i == 3)
                {
                    Assert.AreEqual("8Z5esfhcw6zb9kBRSUH4SWfERoEDUH3cpMXFMnN2F1wC", ix[0].Values.GetValueOrDefault($"Open Orders Account {i + 1}").ToString());
                }
                else
                {
                    Assert.AreEqual(SystemProgram.ProgramIdKey, ix[0].Values.GetValueOrDefault($"Open Orders Account {i + 1}").ToString());
                }
            }

            Assert.AreEqual("Settle Funds", ix[1].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[1].ProgramName);
            Assert.AreEqual(0, ix[1].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[1].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[1].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[1].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z", ix[1].Values.GetValueOrDefault("Mango Cache").ToString());
            Assert.AreEqual("DESVgJVGajEgKGXhb6XmqDHGz3VjdgP7rEVESBgxmroY", ix[1].Values.GetValueOrDefault("Dex Program").ToString());
            Assert.AreEqual("5xWpt56U1NCuHoAEtpLeUrQcxDkEpNfScjfLFaRzLPgR", ix[1].Values.GetValueOrDefault("Spot Market").ToString());
            Assert.AreEqual("EfwvntttcP253P1g8Jqo18Rywyb33deGe69Txn5LSgwQ", ix[1].Values.GetValueOrDefault("Dex Base Vault").ToString());
            Assert.AreEqual("HGsnYZr6yodSHXHNKdtprooS9XP4h19hrzXfgJLuA3MB", ix[1].Values.GetValueOrDefault("Dex Quote Vault").ToString());
            Assert.AreEqual("8GC81raaLjhTx3yedctxCJW46qdmmSRybH2s1eFYFFxT", ix[1].Values.GetValueOrDefault("Base Root Bank").ToString());
            Assert.AreEqual("7mYqCavd1K24fnL3oKTpX3YM66W5gfikmVHJWM3nrWKe", ix[1].Values.GetValueOrDefault("Base Node Bank").ToString());
            Assert.AreEqual("E79n2SiixBFrQqrq8JDCe1ZJXHcVADaQofe246b9qaRy", ix[1].Values.GetValueOrDefault("Base Vault").ToString());
            Assert.AreEqual("HUBX4iwWEUK5VrXXXcB7uhuKrfT4fpu2T9iZbg712JrN", ix[1].Values.GetValueOrDefault("Quote Root Bank").ToString());
            Assert.AreEqual("J2Lmnc1e4frMnBEJARPoHtfpcohLfN67HdK1inXjTFSM", ix[1].Values.GetValueOrDefault("Quote Node Bank").ToString());
            Assert.AreEqual("AV4CuwdvnccZMXNhu9cSCx1mkpgHWcwWEJ7Yb8Xh8QMC", ix[1].Values.GetValueOrDefault("Quote Vault").ToString());
            Assert.AreEqual(TokenProgram.ProgramIdKey, ix[1].Values.GetValueOrDefault("Token Program").ToString());
            Assert.AreEqual("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax", ix[1].Values.GetValueOrDefault("Mango Signer").ToString());
            Assert.AreEqual("8Z5esfhcw6zb9kBRSUH4SWfERoEDUH3cpMXFMnN2F1wC", ix[1].Values.GetValueOrDefault($"Open Orders Account").ToString());
        }

        [TestMethod]
        public void PlaceSpotOrder2SettleFunds()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(PlaceSpotOrder2SettleFundsMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(2, ix.Count);
            Assert.AreEqual("Place Spot Order2", ix[0].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[0].ProgramName);
            Assert.AreEqual(0, ix[0].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[0].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[0].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[0].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z", ix[0].Values.GetValueOrDefault("Mango Cache").ToString());
            Assert.AreEqual("DESVgJVGajEgKGXhb6XmqDHGz3VjdgP7rEVESBgxmroY", ix[0].Values.GetValueOrDefault("Dex Program").ToString());
            Assert.AreEqual("5xWpt56U1NCuHoAEtpLeUrQcxDkEpNfScjfLFaRzLPgR", ix[0].Values.GetValueOrDefault("Spot Market").ToString());
            Assert.AreEqual("8ezpneRznTJNZWFSLeQvtPCagpsUVWA7djLSzqp3Hx4p", ix[0].Values.GetValueOrDefault("Bids").ToString());
            Assert.AreEqual("8gJhxSwbLJkDQbqgzbJ6mDvJYnEVWB6NHWEN9oZZkwz7", ix[0].Values.GetValueOrDefault("Asks").ToString());
            Assert.AreEqual("4hm9fSbga3vosq4K4czPFwy1tmigc4io4ZLJqgC2s4Yv", ix[0].Values.GetValueOrDefault("Dex Request Queue").ToString());
            Assert.AreEqual("48be6VKEq86awgUjfvbKDmEzXr4WNR7hzDxfF6ZPptmd", ix[0].Values.GetValueOrDefault("Dex Event Queue").ToString());
            Assert.AreEqual("EfwvntttcP253P1g8Jqo18Rywyb33deGe69Txn5LSgwQ", ix[0].Values.GetValueOrDefault("Dex Base Vault").ToString());
            Assert.AreEqual("HGsnYZr6yodSHXHNKdtprooS9XP4h19hrzXfgJLuA3MB", ix[0].Values.GetValueOrDefault("Dex Quote Vault").ToString());
            Assert.AreEqual("8GC81raaLjhTx3yedctxCJW46qdmmSRybH2s1eFYFFxT", ix[0].Values.GetValueOrDefault("Base Root Bank").ToString());
            Assert.AreEqual("7mYqCavd1K24fnL3oKTpX3YM66W5gfikmVHJWM3nrWKe", ix[0].Values.GetValueOrDefault("Base Node Bank").ToString());
            Assert.AreEqual("E79n2SiixBFrQqrq8JDCe1ZJXHcVADaQofe246b9qaRy", ix[0].Values.GetValueOrDefault("Base Vault").ToString());
            Assert.AreEqual("HUBX4iwWEUK5VrXXXcB7uhuKrfT4fpu2T9iZbg712JrN", ix[0].Values.GetValueOrDefault("Quote Root Bank").ToString());
            Assert.AreEqual("J2Lmnc1e4frMnBEJARPoHtfpcohLfN67HdK1inXjTFSM", ix[0].Values.GetValueOrDefault("Quote Node Bank").ToString());
            Assert.AreEqual("AV4CuwdvnccZMXNhu9cSCx1mkpgHWcwWEJ7Yb8Xh8QMC", ix[0].Values.GetValueOrDefault("Quote Vault").ToString());
            Assert.AreEqual(TokenProgram.ProgramIdKey, ix[0].Values.GetValueOrDefault("Token Program").ToString());
            Assert.AreEqual("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax", ix[0].Values.GetValueOrDefault("Mango Signer").ToString());
            Assert.AreEqual("21YuRgN6iHgsucfXT6Yzo2dV7dzuAdz2vxExahY3MueT", ix[0].Values.GetValueOrDefault("Dex Vault Signer").ToString());
            Assert.AreEqual("7nS8AgndAVYCfSTaXabwqcBWBc3xCLwwcPiMWzuTbfrf", ix[0].Values.GetValueOrDefault("Serum Vault").ToString());
            Assert.AreEqual("Sell", ix[0].Values.GetValueOrDefault("Side").ToString());
            Assert.AreEqual("105000", ix[0].Values.GetValueOrDefault("Price").ToString());
            Assert.AreEqual("250", ix[0].Values.GetValueOrDefault("Max Coin Quantity").ToString());
            Assert.AreEqual("2625000000", ix[0].Values.GetValueOrDefault("Max Price Coin Quantity").ToString());
            Assert.AreEqual("AbortTransaction", ix[0].Values.GetValueOrDefault("Self Trade Behavior").ToString());
            Assert.AreEqual("ImmediateOrCancel", ix[0].Values.GetValueOrDefault("Order Type").ToString());
            Assert.AreEqual("1000000", ix[0].Values.GetValueOrDefault("Client Order Id").ToString());
            Assert.AreEqual("65535", ix[0].Values.GetValueOrDefault("Limit").ToString());
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                if (i == 3)
                {
                    Assert.AreEqual("8Z5esfhcw6zb9kBRSUH4SWfERoEDUH3cpMXFMnN2F1wC", ix[0].Values.GetValueOrDefault($"Open Orders Account {i + 1}").ToString());
                }
                else
                {
                    Assert.AreEqual(SystemProgram.ProgramIdKey, ix[0].Values.GetValueOrDefault($"Open Orders Account {i + 1}").ToString());
                }
            }

            Assert.AreEqual("Settle Funds", ix[1].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[1].ProgramName);
            Assert.AreEqual(0, ix[1].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[1].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[1].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[1].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z", ix[1].Values.GetValueOrDefault("Mango Cache").ToString());
            Assert.AreEqual("DESVgJVGajEgKGXhb6XmqDHGz3VjdgP7rEVESBgxmroY", ix[1].Values.GetValueOrDefault("Dex Program").ToString());
            Assert.AreEqual("5xWpt56U1NCuHoAEtpLeUrQcxDkEpNfScjfLFaRzLPgR", ix[1].Values.GetValueOrDefault("Spot Market").ToString());
            Assert.AreEqual("EfwvntttcP253P1g8Jqo18Rywyb33deGe69Txn5LSgwQ", ix[1].Values.GetValueOrDefault("Dex Base Vault").ToString());
            Assert.AreEqual("HGsnYZr6yodSHXHNKdtprooS9XP4h19hrzXfgJLuA3MB", ix[1].Values.GetValueOrDefault("Dex Quote Vault").ToString());
            Assert.AreEqual("8GC81raaLjhTx3yedctxCJW46qdmmSRybH2s1eFYFFxT", ix[1].Values.GetValueOrDefault("Base Root Bank").ToString());
            Assert.AreEqual("7mYqCavd1K24fnL3oKTpX3YM66W5gfikmVHJWM3nrWKe", ix[1].Values.GetValueOrDefault("Base Node Bank").ToString());
            Assert.AreEqual("E79n2SiixBFrQqrq8JDCe1ZJXHcVADaQofe246b9qaRy", ix[1].Values.GetValueOrDefault("Base Vault").ToString());
            Assert.AreEqual("HUBX4iwWEUK5VrXXXcB7uhuKrfT4fpu2T9iZbg712JrN", ix[1].Values.GetValueOrDefault("Quote Root Bank").ToString());
            Assert.AreEqual("J2Lmnc1e4frMnBEJARPoHtfpcohLfN67HdK1inXjTFSM", ix[1].Values.GetValueOrDefault("Quote Node Bank").ToString());
            Assert.AreEqual("AV4CuwdvnccZMXNhu9cSCx1mkpgHWcwWEJ7Yb8Xh8QMC", ix[1].Values.GetValueOrDefault("Quote Vault").ToString());
            Assert.AreEqual(TokenProgram.ProgramIdKey, ix[1].Values.GetValueOrDefault("Token Program").ToString());
            Assert.AreEqual("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax", ix[1].Values.GetValueOrDefault("Mango Signer").ToString());
            Assert.AreEqual("8Z5esfhcw6zb9kBRSUH4SWfERoEDUH3cpMXFMnN2F1wC", ix[1].Values.GetValueOrDefault($"Open Orders Account").ToString());
        }

        [TestMethod]
        public void CancelSpotOrder()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(CancelSpotOrderMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, ix.Count);
            Assert.AreEqual("Cancel Spot Order", ix[0].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[0].ProgramName);
            Assert.AreEqual(0, ix[0].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[0].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[0].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[0].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("DESVgJVGajEgKGXhb6XmqDHGz3VjdgP7rEVESBgxmroY", ix[0].Values.GetValueOrDefault("Dex Program").ToString());
            Assert.AreEqual("5xWpt56U1NCuHoAEtpLeUrQcxDkEpNfScjfLFaRzLPgR", ix[0].Values.GetValueOrDefault("Spot Market").ToString());
            Assert.AreEqual("8ezpneRznTJNZWFSLeQvtPCagpsUVWA7djLSzqp3Hx4p", ix[0].Values.GetValueOrDefault("Bids").ToString());
            Assert.AreEqual("8gJhxSwbLJkDQbqgzbJ6mDvJYnEVWB6NHWEN9oZZkwz7", ix[0].Values.GetValueOrDefault("Asks").ToString());
            Assert.AreEqual("8Z5esfhcw6zb9kBRSUH4SWfERoEDUH3cpMXFMnN2F1wC", ix[0].Values.GetValueOrDefault($"Open Orders Account").ToString());
            Assert.AreEqual("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax", ix[0].Values.GetValueOrDefault("Mango Signer").ToString());
            Assert.AreEqual("48be6VKEq86awgUjfvbKDmEzXr4WNR7hzDxfF6ZPptmd", ix[0].Values.GetValueOrDefault("Dex Event Queue").ToString());
            Assert.AreEqual("Sell", ix[0].Values.GetValueOrDefault("Side").ToString());
            Assert.AreEqual("4611686018427387908904197", ix[0].Values.GetValueOrDefault("Order Id").ToString());
        }

        [TestMethod]
        public void PlacePerpOrder()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(PlacePerpOrderMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);
            foreach (var i in ix)
            {
                System.Diagnostics.Debug.WriteLine(i.ToString());
            }

            Assert.AreEqual(1, ix.Count);
            Assert.AreEqual("Place Perp Order", ix[0].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[0].ProgramName);
            Assert.AreEqual(0, ix[0].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[0].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[0].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[0].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z", ix[0].Values.GetValueOrDefault("Mango Cache").ToString());
            Assert.AreEqual("58vac8i9QXStG1hpaa4ouwE1X7ngeDjY9oY7R15hcbKJ", ix[0].Values.GetValueOrDefault("Perp Market").ToString());
            Assert.AreEqual("7HRgm8iXEDx2TmSETo3Lq9SXkF954HMVKNiq8t5sKvQS", ix[0].Values.GetValueOrDefault("Bids").ToString());
            Assert.AreEqual("4oNxXQv1Rx3h7aNWjhTs3PWBoXdoPZjCaikSThV4yGb8", ix[0].Values.GetValueOrDefault("Asks").ToString());
            Assert.AreEqual("CZ5MCRvkN38d5pnZDDEEyMiED3drgDUVpEUjkuJq31Kf", ix[0].Values.GetValueOrDefault("Event Queue").ToString());
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                if (i == 3)
                {
                    Assert.AreEqual("8Z5esfhcw6zb9kBRSUH4SWfERoEDUH3cpMXFMnN2F1wC", ix[0].Values.GetValueOrDefault($"Open Orders Account {i + 1}").ToString());
                }
                else
                {
                    Assert.AreEqual(SystemProgram.ProgramIdKey, ix[0].Values.GetValueOrDefault($"Open Orders Account {i + 1}").ToString());
                }
            }
            Assert.AreEqual("Buy", ix[0].Values.GetValueOrDefault("Side").ToString());
            Assert.AreEqual("11200", ix[0].Values.GetValueOrDefault("Price").ToString());
            Assert.AreEqual("25000", ix[0].Values.GetValueOrDefault("Quantity").ToString());
            Assert.AreEqual("ImmediateOrCancel", ix[0].Values.GetValueOrDefault("Order Type").ToString());
            Assert.AreEqual("1000000", ix[0].Values.GetValueOrDefault("Client Order Id").ToString());
            Assert.AreEqual("False", ix[0].Values.GetValueOrDefault("Reduce Only").ToString());
        }

        [TestMethod]
        public void PlacePerpOrderReduceOnly()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(PlacePerpOrderReduceOnlyMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);
            foreach (var i in ix)
            {
                System.Diagnostics.Debug.WriteLine(i.ToString());
            }

            Assert.AreEqual(1, ix.Count);
            Assert.AreEqual("Place Perp Order", ix[0].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[0].ProgramName);
            Assert.AreEqual(0, ix[0].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[0].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[0].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[0].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z", ix[0].Values.GetValueOrDefault("Mango Cache").ToString());
            Assert.AreEqual("58vac8i9QXStG1hpaa4ouwE1X7ngeDjY9oY7R15hcbKJ", ix[0].Values.GetValueOrDefault("Perp Market").ToString());
            Assert.AreEqual("7HRgm8iXEDx2TmSETo3Lq9SXkF954HMVKNiq8t5sKvQS", ix[0].Values.GetValueOrDefault("Bids").ToString());
            Assert.AreEqual("4oNxXQv1Rx3h7aNWjhTs3PWBoXdoPZjCaikSThV4yGb8", ix[0].Values.GetValueOrDefault("Asks").ToString());
            Assert.AreEqual("CZ5MCRvkN38d5pnZDDEEyMiED3drgDUVpEUjkuJq31Kf", ix[0].Values.GetValueOrDefault("Event Queue").ToString());
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                if (i == 3)
                {
                    Assert.AreEqual("8Z5esfhcw6zb9kBRSUH4SWfERoEDUH3cpMXFMnN2F1wC", ix[0].Values.GetValueOrDefault($"Open Orders Account {i + 1}").ToString());
                }
                else
                {
                    Assert.AreEqual(SystemProgram.ProgramIdKey, ix[0].Values.GetValueOrDefault($"Open Orders Account {i + 1}").ToString());
                }
            }
            Assert.AreEqual("Sell", ix[0].Values.GetValueOrDefault("Side").ToString());
            Assert.AreEqual("10500", ix[0].Values.GetValueOrDefault("Price").ToString());
            Assert.AreEqual("25000", ix[0].Values.GetValueOrDefault("Quantity").ToString());
            Assert.AreEqual("ImmediateOrCancel", ix[0].Values.GetValueOrDefault("Order Type").ToString());
            Assert.AreEqual("1000000", ix[0].Values.GetValueOrDefault("Client Order Id").ToString());
            Assert.AreEqual("True", ix[0].Values.GetValueOrDefault("Reduce Only").ToString());
        }

        [TestMethod]
        public void PlacePerpOrder2WithTimeInForce()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(PlacePerpOrder2WithTifMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, ix.Count);
            Assert.AreEqual("Place Perp Order2", ix[0].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[0].ProgramName);
            Assert.AreEqual(0, ix[0].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[0].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[0].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[0].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z", ix[0].Values.GetValueOrDefault("Mango Cache").ToString());
            Assert.AreEqual("58vac8i9QXStG1hpaa4ouwE1X7ngeDjY9oY7R15hcbKJ", ix[0].Values.GetValueOrDefault("Perp Market").ToString());
            Assert.AreEqual("7HRgm8iXEDx2TmSETo3Lq9SXkF954HMVKNiq8t5sKvQS", ix[0].Values.GetValueOrDefault("Bids").ToString());
            Assert.AreEqual("4oNxXQv1Rx3h7aNWjhTs3PWBoXdoPZjCaikSThV4yGb8", ix[0].Values.GetValueOrDefault("Asks").ToString());
            Assert.AreEqual("CZ5MCRvkN38d5pnZDDEEyMiED3drgDUVpEUjkuJq31Kf", ix[0].Values.GetValueOrDefault("Event Queue").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[0].Values.GetValueOrDefault("Referrer Mango Account").ToString());
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                if (i == 0)
                {
                    Assert.AreEqual("iu1duyU4vcfio6B2rKMToRQQNy4VVDYVGYf7bi8NpNC", ix[0].Values.GetValueOrDefault($"Open Orders Account {i + 1}").ToString());
                }
                else if (i == 3)
                {
                    Assert.AreEqual("8Z5esfhcw6zb9kBRSUH4SWfERoEDUH3cpMXFMnN2F1wC", ix[0].Values.GetValueOrDefault($"Open Orders Account {i + 1}").ToString());
                }
                else
                {
                    Assert.AreEqual(SystemProgram.ProgramIdKey, ix[0].Values.GetValueOrDefault($"Open Orders Account {i + 1}").ToString());
                }
            }
            Assert.AreEqual("7500", ix[0].Values.GetValueOrDefault("Price").ToString());
            Assert.AreEqual("10000", ix[0].Values.GetValueOrDefault("Max Base Quantity").ToString());
            Assert.AreEqual("75000000", ix[0].Values.GetValueOrDefault("Max Quote Quantity").ToString());
            Assert.AreEqual("1", ix[0].Values.GetValueOrDefault("Client Order Id").ToString());
            Assert.AreEqual("1645745263", ix[0].Values.GetValueOrDefault("Expiry Timestamp").ToString());
            Assert.AreEqual("Buy", ix[0].Values.GetValueOrDefault("Side").ToString());
            Assert.AreEqual("Limit", ix[0].Values.GetValueOrDefault("Order Type").ToString());
            Assert.AreEqual("False", ix[0].Values.GetValueOrDefault("Reduce Only").ToString());
            Assert.AreEqual("255", ix[0].Values.GetValueOrDefault("Limit").ToString());
        }

        [TestMethod]
        public void SettleFees()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(SettleFeesMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, ix.Count);
            Assert.AreEqual("Settle Fees", ix[0].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[0].ProgramName);
            Assert.AreEqual(0, ix[0].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[0].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z", ix[0].Values.GetValueOrDefault("Mango Cache").ToString());
            Assert.AreEqual("58vac8i9QXStG1hpaa4ouwE1X7ngeDjY9oY7R15hcbKJ", ix[0].Values.GetValueOrDefault("Perp Market").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[0].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("HUBX4iwWEUK5VrXXXcB7uhuKrfT4fpu2T9iZbg712JrN", ix[0].Values.GetValueOrDefault("Root Bank").ToString());
            Assert.AreEqual("J2Lmnc1e4frMnBEJARPoHtfpcohLfN67HdK1inXjTFSM", ix[0].Values.GetValueOrDefault("Node Bank").ToString());
            Assert.AreEqual("AV4CuwdvnccZMXNhu9cSCx1mkpgHWcwWEJ7Yb8Xh8QMC", ix[0].Values.GetValueOrDefault("Bank Vault").ToString());
            Assert.AreEqual("54PcMYTAZd8uRaYyb3Cwgctcfc1LchGMaqVrmxgr3yVs", ix[0].Values.GetValueOrDefault("Fees Vault").ToString());
            Assert.AreEqual("CFdbPXrnPLmo5Qrze7rw9ZNiD82R1VeNdoQosooSP1Ax", ix[0].Values.GetValueOrDefault("Signer").ToString());
            Assert.AreEqual(TokenProgram.ProgramIdKey, ix[0].Values.GetValueOrDefault("Token Program").ToString());
        }

        [TestMethod]
        public void SetDelegate()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(SetDelegateMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, ix.Count);
            Assert.AreEqual("Set Delegate", ix[0].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[0].ProgramName);
            Assert.AreEqual(0, ix[0].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[0].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[0].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[0].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", ix[0].Values.GetValueOrDefault("Delegate").ToString());
        }

        [TestMethod]
        public void AddPerpTriggerOrder()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(AddPerpTriggerOrderMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, ix.Count);
            Assert.AreEqual("Add Perp Trigger Order", ix[0].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[0].ProgramName);
            Assert.AreEqual(0, ix[0].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[0].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("6iT9xdeMXqytgCpKqicPMJRCDk59mv7GYBSZkQAAP7R5", ix[0].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[0].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("CeUu8EfRAptNPe98pma7RHX6t34C2U25dRhsS3tkRdUB", ix[0].Values.GetValueOrDefault("Advanced Orders").ToString());
            Assert.AreEqual("8mFQbdXsFXt3R3cu3oSNS3bDZRwJRP18vyzd9J278J9z", ix[0].Values.GetValueOrDefault("Mango Cache").ToString());
            Assert.AreEqual("58vac8i9QXStG1hpaa4ouwE1X7ngeDjY9oY7R15hcbKJ", ix[0].Values.GetValueOrDefault("Perp Market").ToString());
            for (int i = 0; i < Constants.MaxPairs; i++)
            {
                Assert.AreEqual(SystemProgram.ProgramIdKey, ix[0].Values.GetValueOrDefault($"Open Orders Account {i + 1}").ToString());
            }
            Assert.AreEqual("Buy", ix[0].Values.GetValueOrDefault("Side").ToString());
            Assert.AreEqual("100000", ix[0].Values.GetValueOrDefault("Price").ToString());
            Assert.AreEqual("100000", ix[0].Values.GetValueOrDefault("Trigger Price").ToString());
            Assert.AreEqual("100000", ix[0].Values.GetValueOrDefault("Quantity").ToString());
            Assert.AreEqual("ImmediateOrCancel", ix[0].Values.GetValueOrDefault("Order Type").ToString());
            Assert.AreEqual("Below", ix[0].Values.GetValueOrDefault("Trigger Condition").ToString());
            Assert.AreEqual("1000000", ix[0].Values.GetValueOrDefault("Client Order Id").ToString());
            Assert.AreEqual("True", ix[0].Values.GetValueOrDefault("Reduce Only").ToString());
        }

        [TestMethod]
        public void RemoveAdvancedOrder()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(RemoveAdvancedOrderMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(2, ix.Count);
            Assert.AreEqual("Remove Advanced Order", ix[0].InstructionName);
            Assert.AreEqual("Mango Program V3", ix[0].ProgramName);
            Assert.AreEqual(0, ix[0].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[0].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("6iT9xdeMXqytgCpKqicPMJRCDk59mv7GYBSZkQAAP7R5", ix[0].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[0].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("CeUu8EfRAptNPe98pma7RHX6t34C2U25dRhsS3tkRdUB", ix[0].Values.GetValueOrDefault("Advanced Orders").ToString());
            Assert.AreEqual(SystemProgram.ProgramIdKey, ix[0].Values.GetValueOrDefault("System Program").ToString());
            Assert.AreEqual("0", ix[0].Values.GetValueOrDefault("Order Index").ToString());
            Assert.AreEqual("Remove Advanced Order", ix[1].InstructionName);
            Assert.AreEqual("1", ix[1].Values.GetValueOrDefault("Order Index").ToString());
        }

        [TestMethod]
        public void CreateAddInfoAndSetReferral()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(CreateAddInfoAndSetReferrerMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(3, ix.Count);
            Assert.AreEqual("Mango Program V3", ix[2].ProgramName);
            Assert.AreEqual("Set Referrer Memory", ix[2].InstructionName);
            Assert.AreEqual(0, ix[2].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[2].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("6hkegFv9bCeKpKJdvhJwbmrKyAAeDS3vkkWC5RHeBWUM", ix[2].Values.GetValueOrDefault("Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[2].Values.GetValueOrDefault("Owner").ToString());
            Assert.AreEqual("5dTNBvZcxPP6p989uXZZZAasUCRYne9x5fnnVLTB3rND", ix[2].Values.GetValueOrDefault("Referrer Memory").ToString());
            Assert.AreEqual("BEPi5vEzwwY5SDfzxWsVQiK7ApuTE3doJkYUVGqAoX2s", ix[2].Values.GetValueOrDefault("Referrer Mango Account").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[2].Values.GetValueOrDefault("Payer").ToString());
            Assert.AreEqual(SystemProgram.ProgramIdKey, ix[2].Values.GetValueOrDefault("System Program").ToString());
        }

        [TestMethod]
        public void RegisterReferrerId()
        {
            Message msg = Message.Deserialize(Convert.FromBase64String(RegisterReferrerIdMessage));
            List<DecodedInstruction> ix =
                InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, ix.Count);
            Assert.AreEqual("Mango Program V3", ix[0].ProgramName);
            Assert.AreEqual("Register Referrer Id", ix[0].InstructionName);
            Assert.AreEqual(0, ix[0].InnerInstructions.Count);
            Assert.AreEqual("Ec2enZyoC4nGpEfu2sUNAa2nUGJHWxoUWYSEJ2hNTWTA", ix[0].Values.GetValueOrDefault("Mango Group").ToString());
            Assert.AreEqual("6iT9xdeMXqytgCpKqicPMJRCDk59mv7GYBSZkQAAP7R5", ix[0].Values.GetValueOrDefault("Referrer Mango Account").ToString());
            Assert.AreEqual("Abs9roAyf35gb7xf7FTMoxfGviLMGQpMdVhydpDmcJFR", ix[0].Values.GetValueOrDefault("Referrer Id Record").ToString());
            Assert.AreEqual("hoakwpFB8UoLnPpLC56gsjpY7XbVwaCuRQRMQzN5TVh", ix[0].Values.GetValueOrDefault("Payer").ToString());
            Assert.AreEqual(SystemProgram.ProgramIdKey, ix[0].Values.GetValueOrDefault("System Program").ToString());
            Assert.AreEqual("\v\0\0\0\0\0\0\0Mango Sharp", ix[0].Values.GetValueOrDefault("Referrer Id").ToString());
        }
    }
}
