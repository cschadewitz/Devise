using Cottle;

namespace Devise.Utilities
{
    public static class CottleRenderer
    {
        public static string Render(string cottleTemplate, IContext entityContext)
        {
            var configuration = new DocumentConfiguration
            {
                Trimmer = DocumentConfiguration.TrimNothing
            };

            var cottleDocumentResult = Document.CreateDefault(cottleTemplate, configuration);
            IDocument cottleDocument = cottleDocumentResult.DocumentOrThrow;
            return cottleDocument.Render(entityContext);
        }
    }
}
