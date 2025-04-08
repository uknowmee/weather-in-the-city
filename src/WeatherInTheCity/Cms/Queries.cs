using System.Text.Json.Serialization;

namespace WeatherInTheCity.Cms;

public class Queries
{
    public record GetAppAboutResponse([property: JsonPropertyName("about")] About AppAbout);
    public const string GetAppAbout = """
                                      query GetAbout($name: String!) {
                                        about(where: {name: $name}) {
                                          name
                                          description {
                                            html
                                          }
                                          features {
                                            html
                                          }
                                        }
                                      }
                                      """;

    public record GetProjectsResponse([property: JsonPropertyName("projects")] List<Project> Projects);
    public const string GetProjects = """
                                      query GetProjects {
                                        projects {
                                          name
                                          year
                                          gitLink
                                          content {
                                            html
                                          }
                                          img(locales: en) {
                                            url
                                            width
                                            height
                                            placeholder: url(
                                              transformation: {
                                                image: { resize: { width: 10, height: 10, fit: clip } }
                                              }
                                            )
                                          }
                                        }
                                      }
                                      """;
}