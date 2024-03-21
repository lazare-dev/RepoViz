// api/pkg/analyzer/detectLanguage.go

package analyzer

import (
	"log"
	"os"
	"path/filepath"

	"github.com/go-enry/go-enry/v2"
)

type LanguageDetails struct {
	Count     int
	FileNames []string
}

func DetectLanguagesInRepo(directory string) (map[string]*LanguageDetails, error) {
	log.Printf("Starting language detection in directory: %s\n", directory)
	languages := make(map[string]*LanguageDetails)

	err := filepath.Walk(directory, func(path string, info os.FileInfo, err error) error {
		if err != nil {
			log.Printf("Encountered an error accessing path: %s, error: %v\n", path, err)
			return err
		}
		if !info.IsDir() {
			content, readErr := os.ReadFile(path)
			if readErr != nil {
				log.Printf("Failed to read file: %s, error: %v\n", path, readErr)
				return readErr
			}
			language, _ := enry.GetLanguageByContent(path, content)
			if detail, exists := languages[language]; !exists {
				languages[language] = &LanguageDetails{Count: 1, FileNames: []string{path}}
				log.Printf("Detected new language: %s, file: %s\n", language, path)
			} else {
				detail.Count++
				detail.FileNames = append(detail.FileNames, path)
			}
		}
		return nil
	})

	if err != nil {
		log.Printf("Failed to complete language detection, error: %v\n", err)
		return nil, err
	}

	log.Printf("Completed language detection. Total languages detected: %d\n", len(languages))
	return languages, nil
}
